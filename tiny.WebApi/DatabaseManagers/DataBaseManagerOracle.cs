// <copyright file="DataBaseManagerOracle.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
using tiny.WebApi.Helpers;
using tiny.WebApi.IDBContext;
namespace tiny.WebApi.DatabaseManagers
{
    /// <summary>
    ///     A data base manager oracle.
    /// </summary>
    /// <seealso cref="T:IDisposable"/>
    [DebuggerStepThrough]
    public class DataBaseManagerOracle : IDisposable
    {
        /// <summary>
        ///     (Immutable) the context.
        /// </summary>
        private readonly IDBContextOracle _context;
        /// <summary>
        ///     The connection.
        /// </summary>
        private OracleConnection _conn;
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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private OracleTransaction Trans { get; set; } = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Gets the transaction.
        /// </summary>
        /// <value>
        ///     The transaction.
        /// </value>
        public OracleTransaction Transaction => Trans;
        /// <summary>
        ///     (Immutable) true to automatically dispose connection.
        /// </summary>
        private readonly bool _autoDisposeConnection = false;
        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool _disposed;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.DatabaseManagers.DataBaseManagerOracle class.
        /// </summary>
        /// <param name="context">               (Immutable) the context. </param>
        /// <param name="querySpecification">    (Immutable) the query specification. </param>
        /// <param name="autoDisposeConnection">
        /// (Optional)
        /// (Immutable) true to automatically dispose connection. 
        /// Auto disposition of connections will work only with close database architecture calls that is not with ExecuteReader and ExecuteXMLReader also only when Transaction is not required.
        /// </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataBaseManagerOracle(IDBContextOracle context, QuerySpecification querySpecification, bool autoDisposeConnection = true)
        {
            Global.LogDebug("Inside DataBaseManagerOracle and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _context.AutoDisposeConnection = _autoDisposeConnection = autoDisposeConnection;
            _context.Transaction = Transaction;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.DatabaseManagers.DataBaseManagerOracle class.
        /// </summary>
        /// <param name="context">               (Immutable) the context. </param>
        /// <param name="connectionString">      The connection string. </param>
        /// <param name="autoDisposeConnection">
        /// (Optional)
        /// (Immutable) true to automatically dispose connection. default is false.
        ///  Auto disposition of connections will work only with close database architecture calls that is not with ExecuteReader and ExecuteXMLReader also only when Transaction is not required.
        /// </param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DataBaseManagerOracle(IDBContextOracle context, string connectionString, bool autoDisposeConnection = false)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Global.LogDebug("Inside DataBaseManagerOracle and setting up the parameters.");
            _context = context;
            _context.AutoDisposeConnection = _autoDisposeConnection = autoDisposeConnection;
            _context.Transaction = Transaction;
            _conn = _context.GetConnection(connectionString + "", false);
        }
        /// <summary>
        ///     Creates a command.
        /// </summary>
        /// <remarks>
        ///     When the DatabaseParameterType is Structured that is complex UDT and not an Out Parameter: If isMapUDTAsXML or IsMapUDTAsJSON are true then the UDT will be mapped serialized as XML/JSON and mapped as CLOB. else it UDT will be mapped as UDT but will only be supported with Oracle 21C onwards.
        ///     When the DatabaseParameterType is Binary and not an Out Parameterthen the parameter will be mapped as BLOB.
        ///     The type of UDT (Supported for Oracle 21C and above) and type information should be availed vai Tag field of DatabaseParameters.
        /// <param name="query">          The query. </param>
        /// <param name="type">           The type. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     The new command.
        /// </returns>
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleCommand CreateCommand(string query, CommandType type, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            Global.LogDebug("Inside CreateCommand.");
            OracleCommand cmd = new(query, _conn);
            Global.LogDebug("If transaction is not null then set transaction.");
            if (Trans is not null) cmd.Transaction = Trans;
            Global.LogDebug("Setting command type, command timeout.");
            cmd.BindByName = true;
            cmd.CommandType = type;
            cmd.CommandTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            Global.LogDebug("Set parameters if any.");
            if (parameters is not null && parameters.Count > 0)
            {
                Global.LogDebug("Loop through parameters.");
                foreach (var item in parameters)
                {
                    switch (item.Type)
                    {
                        case DatabaseParameterType.Structured when !item.IsOutParameter:
                            {
                                Global.LogDebug("When parameter type is stuctured and is not an out parameter.");
                                if (isMapUDTAsJSON || isMapUDTAsXML)
                                {
                                    Global.LogDebug("When isMapUDTAsJSON or isMapUDTAsXML is true.");
                                    if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                                    Global.LogDebug("Setting up clob object.");
                                    OracleClob clob = new(_conn);
#pragma warning disable CS8604 // Possible null reference argument.
                                    var arr = Encoding.Unicode.GetBytes(item.Value as string);
#pragma warning restore CS8604 // Possible null reference argument.
                                    clob.Write(arr, 0, arr.Length);
                                    _ = cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(':') ? item.Name.Replace(":", "") : item.Name, OracleDbType.Clob, clob, ParameterDirection.Input);
                                }
                                else
                                {
                                    Global.LogDebug("Setting up UDT in case of 21C and isMapUDTAsJSON or isMapUDTAsXML is false.");
                                    var p = cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(':') ? CommandType.Text == type ? item.Name : ":" + item.Name : CommandType.StoredProcedure == type ? item.Name.Replace(":", "") : item.Name, OracleDbType.Object, item.Value, ParameterDirection.Input);
                                    p.UdtTypeName = item.Tag;
                                }
                                break;
                            }
                        case DatabaseParameterType.Binary when !item.IsOutParameter:
                            {
                                Global.LogDebug("When parameter tpe is binary and is not an out parameter.");
                                if (_conn.State != ConnectionState.Open) { _conn.Open(); }
                                Global.LogDebug("Setting up bindary object.");
                                OracleBlob blob = new(_conn);
#pragma warning disable CS8604 // Possible null reference argument.
                                var arr = Encoding.Unicode.GetBytes(item.Value as string);
#pragma warning restore CS8604 // Possible null reference argument.
                                blob.Write(arr, 0, arr.Length);
                                _ = cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(':') ? item.Name.Replace(":", "") : item.Name, OracleDbType.Blob, blob, ParameterDirection.Input);
                                break;
                            }
                        default:
                            {
                                Global.LogDebug("When parameter tpe is not binary and clob.");
                                var p = cmd.Parameters.Add(new OracleParameter());
                                if (item.IsOutParameter)
                                {
                                    Global.LogDebug("When parameter is an out parameter then set the direction accordingly and DBType as RefCursor.");
                                    p.Direction = ParameterDirection.Output;
                                    p.Size = item.Size is > 0 ? item.Size : p.Size;
                                    if (item.Type == DatabaseParameterType.RefCursor)
                                        p.OracleDbType = OracleDbType.RefCursor;
                                }
                                else
                                {
                                    Global.LogDebug("When parameter is an input parameter then set the direction and value.");
                                    p.Value = item.Value; p.Direction = ParameterDirection.Input;
                                }
                                Global.LogDebug("Setting parameter type and name and ignore the UnKnown, RefCursour DBTypes as they are already handled.");
                                p.ParameterName = !string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(':') ? CommandType.Text == type ? item.Name : ":" + item.Name : CommandType.StoredProcedure == type ? item.Name.Replace(":", "") : item.Name;
                                if (item.Type is not null && item.Type.HasValue && item.Type.Value != DatabaseParameterType.UnKnown && item.Type.Value != DatabaseParameterType.RefCursor)
                                    p.DbType = (DbType)(int)item.Type.Value;
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQuery(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Non Query as text.");
                return _context.ExecuteNonQuery(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Non Query as text with command out.");
                return _context.ExecuteNonQuery(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Non Query as procedure.");
                return _context.ExecuteNonQuery(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Non Query with command out as procedure.");
                return _context.ExecuteNonQuery(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalar(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Scalar as text.");
                return _context.ExecuteScalar(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Scalar as text with command out.");
                return _context.ExecuteScalar(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Scalar as procedure.");
                return _context.ExecuteScalar(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Scalar as procedure with command out.");
                return _context.ExecuteScalar(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDataReader ExecDataReader(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Data Reader as text.");
                return _context.ExecuteDataReader(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDataReader ExecDataReaderWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Data Reader as text with command out.");
                return _context.ExecuteDataReader(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDataReader ExecDataReaderProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Data Reader as procedure.");
                return _context.ExecuteDataReader(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDataReader ExecDataReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Data Reader as procedure with command out.");
                return _context.ExecuteDataReader(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReader(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing XML Reader as text.");
                return _context.ExecuteXmlReader(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing XML Reader as text with command out.");
                return _context.ExecuteXmlReader(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing XML Reader as procedure.");
                return _context.ExecuteXmlReader(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing XML Reader as procedure with command out.");
                return _context.ExecuteXmlReader(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet ExecDataSet(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Set as text.");
                return _context.FillDataSet(new OracleDataAdapter(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Set as text with command out.");
                return _context.FillDataSet(new OracleDataAdapter(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Set as procedure.");
                return _context.FillDataSet(new OracleDataAdapter(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Set as procedure with command out.");
                return _context.FillDataSet(new OracleDataAdapter(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable ExecDataTable(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Table as text.");
                return _context.FillDataTable(new OracleDataAdapter(CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable ExecDataTableWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Table as text with command out.");
                return _context.FillDataTable(new OracleDataAdapter(command = CreateCommand(query, CommandType.Text, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable ExecDataTableProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Table as procedure.");
                return _context.FillDataTable(new OracleDataAdapter(CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="query">          The query. </param>
        /// <param name="parameters">     Options for controlling the operation. </param>
        /// <param name="isMapUDTAsJSON"> True if is map udt as json, false if not. </param>
        /// <param name="isMapUDTAsXML">  True if is map udt as xml, false if not. </param>
        /// <param name="command">        [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable ExecDataTableProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
                Global.LogDebug("Executing Fill Data Table as procedure with command out.");
                return _context.FillDataTable(new OracleDataAdapter(command = CreateCommand(query, CommandType.StoredProcedure, parameters, isMapUDTAsJSON, isMapUDTAsXML)));
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
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQuery(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQuery(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'non query with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecNonQueryWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecNonQueryWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'non query procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQueryProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'non query procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecNonQueryProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalar(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalar(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'scalar with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecScalarWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'scalar procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalarProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'scalar procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecScalarProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecScalarProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReader(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'data reader with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataReaderWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'data reader procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReaderProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'data reader procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataReaderProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReader(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'xml reader with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecXmlReaderWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'xml reader procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReaderProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle);
        /// <summary>
        ///     Executes the 'xml reader procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        ///
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecXmlReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecXmlReaderProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command);
        /// <summary>
        ///     Executes the 'data set' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSet(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => SetInt64ColumnType(ExecDataSetWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out OracleCommand oracleCommand), querySpecification, oracleCommand);
        /// <summary>
        ///     Executes the 'data set with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => SetInt64ColumnType(ExecDataSetWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command), querySpecification, command);
        /// <summary>
        ///     Executes the 'data set procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => SetInt64ColumnType(ExecDataSetProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out OracleCommand oracleCommand), querySpecification, oracleCommand);
        /// <summary>
        ///     Executes the 'data set procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSetProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => SetInt64ColumnType(ExecDataSetProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command), querySpecification, command);
        /// <summary>
        ///     Executes the 'data table' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTable(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => SetInt64ColumnType(ExecDataTableWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out OracleCommand oracleCommand), querySpecification, oracleCommand);
        /// <summary>
        ///     Executes the 'data table with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => SetInt64ColumnType(ExecDataTableWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command), querySpecification, command);
        /// <summary>
        ///     Executes the 'data table procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => SetInt64ColumnType(ExecDataTableProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out OracleCommand oracleCommand), querySpecification, oracleCommand);
        /// <summary>
        ///     Executes the 'data table procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => SetInt64ColumnType(ExecDataTableProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON_ApplicableForOracle, querySpecification.IsMapUDTAsXML_ApplicableForOracle, out command), querySpecification, command);
        /// <summary>
        /// Set Int64 as column type for specified columns in Data Table as Oracle does not differentiate between intergers and decimals and makes every number column as decimal.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="querySpecification"></param>
        /// <param name="oracleCommand"></param>
        /// <returns></returns>
        private static DataTable SetInt64ColumnType(DataTable dt, QuerySpecification querySpecification, OracleCommand oracleCommand)
        {
            try
            {
                if (dt is not null && dt.Columns.Count > 0)
                {
                    var dt1 = dt.Clone();
                    var outPut = querySpecification.Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL;
                    var rcs = outPut.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (rcs is not null && rcs.Length > 0)
                    {
                        var fields = rcs[0].Split(':');
                        if (fields is not null && fields.Length > 0)
                            if (oracleCommand.CommandType == CommandType.Text)
                                foreach (var dataColumn in from item in fields from dataColumn in from DataColumn dataColumn in dt1.Columns where $"{dataColumn.ColumnName}".ToLower() == $"{item}".ToLower() select dataColumn select dataColumn)
                                    dataColumn.DataType = typeof(long);
                            else if (oracleCommand.CommandType == CommandType.StoredProcedure)
                                foreach (var dataColumn in from item in fields.Skip(1) from dataColumn in from DataColumn dataColumn in dt1.Columns where $"{dataColumn.ColumnName}".ToLower() == $"{item}".ToLower() select dataColumn select dataColumn)
                                    dataColumn.DataType = typeof(long);
                    }
                    foreach (DataRow row in dt.Rows)
                        dt1.ImportRow(row);
                    dt.Dispose();
                    return dt1;
                }
            }
            catch { }
#pragma warning disable CS8603 // Possible null reference return.
            return dt;
#pragma warning restore CS8603 // Possible null reference return.
        }
        /// <summary>
        /// Set Int64 as column type for specified columns in Data Set as Oracle does not differentiate between intergers and decimals and makes every number column as decimal.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="querySpecification"></param>
        /// <param name="oracleCommand"></param>
        /// <returns></returns>
        private static DataSet SetInt64ColumnType(DataSet ds, QuerySpecification querySpecification, OracleCommand oracleCommand)
        {
            try
            {
                if (ds is not null && ds.Tables.Count > 0)
                {
                    var ds1 = ds.Clone();
                    var outPut = querySpecification.Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL;
                    var rcs = outPut.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (rcs is not null && rcs.Length > 0)
                    {
                        if (ds1.Tables.Count == rcs.Length)
                        {
                            int level = 0;
                            foreach (DataTable dt in ds1.Tables)
                            {
                                if (dt.Columns.Count > 0)
                                {
                                    var fields = rcs[level].Split(':');
                                    if (fields is not null && fields.Length > 0)
                                        if (oracleCommand.CommandType == CommandType.Text)
                                            foreach (var dataColumn in from item in fields from dataColumn in from DataColumn dataColumn in dt.Columns where $"{dataColumn.ColumnName}".ToLower() == $"{item}".ToLower() select dataColumn select dataColumn)
                                                dataColumn.DataType = typeof(long);
                                        else if (oracleCommand.CommandType == CommandType.StoredProcedure)
                                            foreach (var dataColumn in from item in fields.Skip(1) from dataColumn in from DataColumn dataColumn in dt.Columns where $"{dataColumn.ColumnName}".ToLower() == $"{item}".ToLower() select dataColumn select dataColumn)
                                                dataColumn.DataType = typeof(long);
                                }
                                level++;
                            }
                        }
                    }
                    int lev = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        foreach (DataRow row in ds.Tables[lev].Rows)
                            ds1.Tables[lev].ImportRow(row);
                        lev++;
                    }
                    ds.Dispose();
                    return ds1;
                }
            }
            catch { }
#pragma warning disable CS8603 // Possible null reference return.
            return ds;
#pragma warning restore CS8603 // Possible null reference return.
        }
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:IDisposable.Dispose()"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Dispose()
        {
            Global.LogDebug("Inside Dispose.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        protected virtual void Dispose(bool disposing)
        {
            Global.LogDebug("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
                Rollback();
                Global.LogDebug("When disposing is true and connection is not null.");
                if (disposing && _conn is not null)
                {
                    Global.LogDebug("Lock when disposing connection.");
                    lock (_lockObject)
                    {
                        Global.LogDebug("Close connection when open, dispose and set as null.");
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        _conn = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                    Global.LogDebug("Releasing lock.");
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
            Global.LogDebug("Inside rollback, rollback if transaction is not null.");
            if (Trans is not null)
            {
                Global.LogDebug("Rolling back transaction.");
                Trans.Rollback();
                Global.LogDebug("Transaction rolled back.");
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _context.Transaction = Trans = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }
        /// <summary>
        ///     Commits this object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Commit()
        {
            Global.LogDebug("Inside Commit transaction, commiting if transaction is not null.");
            if (Trans is not null)
            {
                Global.LogDebug("Commiting transaction.");
                Trans.Commit();
                Global.LogDebug("Transaction committed.");
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _context.Transaction = Trans = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }
        /// <summary>
        ///     Begins a transaction.
        /// </summary>
        /// <returns>
        ///     An OracleTransaction.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleTransaction BeginTransaction()
        {
            Global.LogDebug("Inside Begin transaction.");
            Rollback();
            Global.LogDebug("Beginning transaction.");
#pragma warning disable CS8601 // Possible null reference assignment.
            _context.Transaction = Trans = _conn?.BeginTransaction(IsolationLevel.ReadUncommitted);
#pragma warning restore CS8601 // Possible null reference assignment.
            Global.LogDebug("Transaction begun.");
            return Transaction;
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
