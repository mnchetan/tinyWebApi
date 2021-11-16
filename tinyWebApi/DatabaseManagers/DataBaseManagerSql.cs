// <copyright file="DataBaseManagerSql.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataBaseManagerSql(IDBContextSql context, QuerySpecification querySpecification, bool autoDisposeConnection = true)
        {
            Global.LogInformation("Inside DataBaseManagerSql and setting up the parameters.");
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
            Global.LogInformation("Inside DataBaseManagerSql and setting up the parameters.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlCommand CreateCommand(string query, CommandType type, List<DatabaseParameters> parameters)
        {
            Global.LogInformation("Inside CreateCommand.");
            SqlCommand cmd = new(query, _conn);
            Global.LogInformation("If transaction is not null then set transaction.");
            if (Trans is not null) cmd.Transaction = Trans;
            Global.LogInformation("Setting command type, command timeout.");
            cmd.CommandType = type;
            cmd.CommandTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            Global.LogInformation("Set parameters if any.");
            if (parameters is not null && parameters.Count > 0)
            {
                Global.LogInformation("Loop through parameters.");
                foreach (var item in parameters)
                {
                    switch (item.Type)
                    {
                        case DatabaseParameterType.Structured when !item.IsOutParameter:
                            {
                                Global.LogInformation("When parameter tpe is stuctured and is not an out parameter.");
                                var s = cmd.Parameters.AddWithValue(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains('@') ? item.Name : "@" + item.Name, item.Value);
                                s.TypeName = item.Tag;
                                s.SqlDbType = SqlDbType.Structured;
                                break;
                            }
                        default:
                            {
                                var p = cmd.Parameters.Add(new SqlParameter());
                                if (item.IsOutParameter)
                                {
                                    Global.LogInformation("When parameter is an out parameter then set the direction accordingly.");
                                    p.Direction = ParameterDirection.Output;
                                    p.Size = item.Size is > 0 ? item.Size : p.Size;
                                }
                                else
                                {
                                    Global.LogInformation("When parameter is an input parameter then set the direction and value.");
                                    p.Value = item.Value;
                                    p.Direction = ParameterDirection.Input;
                                }
                                Global.LogInformation("Setting parameter type and name and ignore the UnKnown as already handled.");
                                p.ParameterName = !string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains('@') ? item.Name : "@" + item.Name;
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQuery(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Non Query as text.");
                return _context.ExecuteNonQuery(CreateCommand(query, CommandType.Text, parameters));
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
        ///     Executes the 'non query with command out' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <param name="command">    [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Non Query as text with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Non Query as procedure.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Non Query with command out as procedure.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalar(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Scalar as text.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Scalar as text with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Scalar as procedure.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Scalar as procedure with command out.");
                return _context.ExecuteScalar(command = CreateCommand(query, CommandType.StoredProcedure, parameters));
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
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDataReader ExecDataReader(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Data Reader as text.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDataReader ExecDataReaderWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Data Reader as text with command out.");
                return _context.ExecuteDataReader(command = CreateCommand(query, CommandType.Text, parameters));
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
        ///     Executes the 'data reader procedure' operation.
        /// </summary>
        /// <param name="query">      The query. </param>
        /// <param name="parameters"> Options for controlling the operation. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDataReader ExecDataReaderProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Data Reader as procedure.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDataReader ExecDataReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Data Reader as procedure with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReader(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing XML Reader as text.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing XML Reader as text with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing XML Reader as procedure.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing XML Reader as procedure with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet ExecDataSet(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Fill Data Set as text.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet ExecDataSetWithCommandOut(string query, List<DatabaseParameters> parameters, out SqlCommand command)
        {
            try
            {
                Global.LogInformation("Executing Fill Data Set as text with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet ExecDataSetProc(string query, List<DatabaseParameters> parameters)
        {
            try
            {
                Global.LogInformation("Executing Fill Data Set as procedure.");
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
                Global.LogInformation("Executing Fill Data Set as procedure with command out.");
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
                Global.LogInformation("Executing Fill Data Table as text.");
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
                Global.LogInformation("Executing Fill Data Table as text with command out.");
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
                Global.LogInformation("Executing Fill Data Table as procedure.");
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
                Global.LogInformation("Executing Fill Data Table as procedure with command out.");
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
            Global.LogInformation("Inside Dispose.");
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
            Global.LogInformation("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
                Rollback();
                Global.LogInformation("When disposing is true and connection is not null.");
                if (disposing && _conn is not null)
                {
                    Global.LogInformation("Lock when disposing connection.");
                    lock (_lockObject)
                    {
                        Global.LogInformation("Close connection when open, dispose and set as null.");
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
                        _conn = null;
                    }
                    Global.LogInformation("Releasing lock.");
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
            Global.LogInformation("Inside rollback, rollback if transaction is not null.");
            if (Trans is not null)
            {
                Global.LogInformation("Rolling back transaction.");
                Trans.Rollback();
                Global.LogInformation("Transaction rolled back.");
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
            Global.LogInformation("Inside Commit transaction, commiting if transaction is not null.");
            if (Trans is not null)
            {
                Global.LogInformation("Commiting transaction.");
                Trans.Commit();
                Global.LogInformation("Transaction committed.");
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
            Global.LogInformation("Inside Begin transaction.");
            Rollback();
            Global.LogInformation("Beginning transaction.");
            _context.Transaction = Trans = _conn?.BeginTransaction(IsolationLevel.ReadUncommitted);
            Global.LogInformation("Transaction begun.");
            return Transaction;
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
