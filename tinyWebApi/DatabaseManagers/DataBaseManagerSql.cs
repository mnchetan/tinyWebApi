/// <copyright file="DataBaseManagerSql.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.IDBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
namespace tinyWebApi.Common.DatabaseManagers
{
    /// <summary>
    ///     A data base manager sql.
    /// </summary>
    /// <seealso cref="T:IDisposable"/>
    [DebuggerStepThrough]
    public class DataBaseManagerSql : IDisposable
    {
        /// <summary>
        ///     (Immutable) the context.
        /// </summary>
        private readonly IDBContextSql _context;
        /// <summary>
        ///     The connection.
        /// </summary>
        private SqlConnection _conn;
        /// <summary>
        ///     (Immutable) the query specification.
        /// </summary>
        private readonly QuerySpecification _querySpecification;
        /// <summary>
        ///     Gets or sets the transaction.
        /// </summary>
        /// <value>
        ///     The transaction.
        /// </value>
        private SqlTransaction Trans { get; set; } = null;
        /// <summary>
        ///     Gets the transaction.
        /// </summary>
        /// <value>
        ///     The transaction.
        /// </value>
        public SqlTransaction Transaction => Trans;
        /// <summary>
        ///     (Immutable) true to automatically dispose connection.
        /// </summary>
        private readonly bool _autoDisposeConnection = false;
        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.DatabaseManagers.DataBaseManagerSql class.
        /// </summary>
        /// <param name="context">               The context. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="autoDisposeConnection"> (Optional) True to automatically dispose connection. 
        /// </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataBaseManagerSql(IDBContextSql context, QuerySpecification querySpecification, bool autoDisposeConnection = true)
        {
            _context = context;
            _querySpecification = querySpecification;
            _context.AutoDisposeConnection = _autoDisposeConnection = autoDisposeConnection;
            _context.Transaction = Transaction;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.DatabaseManagers.DataBaseManagerSql class.
        /// </summary>
        /// <param name="context">               The context. </param>
        /// <param name="connectionString">      The connection string. </param>
        /// <param name="autoDisposeConnection"> (Optional) True to automatically dispose connection. default is false.
        ///                                       Auto disposition of connections will work only with close database architecture calls that is not with ExecuteReader and ExecuteXMLReader also only when Transaction is not required.
        /// </param>
        public DataBaseManagerSql(IDBContextSql context, string connectionString, bool autoDisposeConnection = false)
        {
            _context = context;
            _context.AutoDisposeConnection = _autoDisposeConnection = autoDisposeConnection;
            _context.Transaction = Transaction;
            _conn = _context.GetConnection(connectionString + "", false);
        }
        /// <summary>
        ///     Creates a command.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="type">       The type. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     The new command.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlCommand CreateCommand(string query, CommandType type, List<DatabaseParameters> parameters)
        {
            SqlCommand cmd = new(query, _conn);
            if (Trans is not null) cmd.Transaction = Trans;
            cmd.CommandType = type;
            cmd.CommandTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            if (parameters is not null && parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    switch (item.Type)
                    {
                        case DatabaseParameterType.Structured when !item.IsOutParameter:
                            {
                                var s = cmd.Parameters.AddWithValue(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains("@") ? item.Name : "@" + item.Name, item.Value);
                                s.TypeName = item.Tag;
                                s.SqlDbType = SqlDbType.Structured;
                                break;
                            }
                        default:
                            {
                                var p = cmd.Parameters.Add(new SqlParameter());
                                if (item.IsOutParameter)
                                {
                                    p.Direction = ParameterDirection.Output;
                                    p.Size = item.Size is > 0 ? item.Size : p.Size;
                                }
                                else
                                {
                                    p.Value = item.Value;
                                    p.Direction = ParameterDirection.Input;
                                }
                                p.ParameterName = !string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains("@") ? item.Name : "@" + item.Name;
                                if (item.Type is not null && item.Type.HasValue && item.Type.Value != DatabaseParameterType.UnKnown) p.DbType = (DbType)(int)item.Type.Value;
                                break;
                            }
                    }
                }
            }
            return cmd;
        }
        /// <summary>
        ///     Executes the 'non query' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQuery(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteNonQuery(CreateCommand(query, CommandType.Text, parameters));
            }
            catch { throw; }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'non query with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteNonQuery(command = CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'non query procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteNonQuery(CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'non query procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteNonQuery(command = CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalar(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteScalar(CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'scalar with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteScalar(command = CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'scalar procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteScalar(CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'scalar procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try { return _context.ExecuteScalar(command = CreateCommand(query, CommandType.StoredProcedure, parameters)); }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReader(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteDataReader(CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data reader with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try { return _context.ExecuteDataReader(command = CreateCommand(query, CommandType.Text, parameters)); }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data reader procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteDataReader(CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data reader procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteDataReader(command = CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReader(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteXmlReader(CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'xml reader with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteXmlReader(command = CreateCommand(query, CommandType.Text, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'xml reader procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.ExecuteXmlReader(CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'xml reader procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.ExecuteXmlReader(command = CreateCommand(query, CommandType.StoredProcedure, parameters));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data set' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSet(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.FillDataSet(new SqlDataAdapter(CreateCommand(query, CommandType.Text, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data set with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.FillDataSet(new SqlDataAdapter(command = CreateCommand(query, CommandType.Text, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data set procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.FillDataSet(new SqlDataAdapter(CreateCommand(query, CommandType.StoredProcedure, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data set procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.FillDataSet(new SqlDataAdapter(command = CreateCommand(query, CommandType.StoredProcedure, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data table' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTable(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.FillDataTable(new SqlDataAdapter(CreateCommand(query, CommandType.Text, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data table with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.FillDataTable(new SqlDataAdapter(command = CreateCommand(query, CommandType.Text, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data table procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                return _context.FillDataTable(new SqlDataAdapter(CreateCommand(query, CommandType.StoredProcedure, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'data table procedure with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                return _context.FillDataTable(new SqlDataAdapter(command = CreateCommand(query, CommandType.StoredProcedure, parameters)));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && _autoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'non query' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQuery(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQuery(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'non query with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecNonQueryWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'non query procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQueryProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'non query procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecNonQueryProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalar(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalar(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'scalar with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecScalarWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'scalar procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalarProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'scalar procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecScalarProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReader(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data reader with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataReaderWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data reader procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReaderProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data reader procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlDataReader ExecDataReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataReaderProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReader(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'xml reader with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecXmlReaderWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'xml reader procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReaderProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'xml reader procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecXmlReaderProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data set' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSet(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataSet(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data set with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataSetWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data set procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataSetProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data set procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataSetProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data table' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTable(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataTable(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data table with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataTableWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Executes the 'data table procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataTableProc(querySpecification.Query, parameters);
        /// <summary>
        ///     Executes the 'data table procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out SqlCommand command) => ExecDataTableProcWithCommandOut(querySpecification.Query, parameters, out command);
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:IDisposable.Dispose()"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        ///     Releases the unmanaged resources used by the tinyWebApi.Common.DatabaseManagers.DataBaseManagerSql and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Rollback();
                if (disposing && _conn is not null)
                {
                    lock (_lockObject)
                    {
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
                        _conn = null;
                    }
                }
                _disposed = true;
            }
        }
        /// <summary>
        ///     Rollbacks this object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Rollback()
        {
            if (Trans is not null)
            {
                Trans.Rollback();
                _context.Transaction = Trans = null;
            }
        }
        /// <summary>
        ///     Commits this object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Commit()
        {
            if (Trans is not null)
            {
                Trans.Commit();
                _context.Transaction = Trans = null;
            }
        }
        /// <summary>
        ///     Begins a transaction.
        /// </summary>
        /// <returns>
        ///     A SqlTransaction.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public SqlTransaction BeginTransaction()
        {
            Rollback();
            _context.Transaction = Trans = _conn?.BeginTransaction(IsolationLevel.ReadUncommitted);
            return Transaction;
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
