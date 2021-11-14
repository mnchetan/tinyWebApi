﻿/// <copyright file="DataBaseManagerOracle.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.Helpers;
using tinyWebApi.Common.IDBContext;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml;
namespace tinyWebApi.Common.DatabaseManagers
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
        private OracleTransaction Trans { get; set; } = null;
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
        private bool _disposed;
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.DatabaseManagers.DataBaseManagerOracle class.
        /// </summary>
        /// <param name="context">               (Immutable) the context. </param>
        /// <param name="querySpecification">    (Immutable) the query specification. </param>
        /// <param name="autoDisposeConnection">
        /// (Optional)
        /// (Immutable) true to automatically dispose connection. 
        /// Auto disposition of connections will work only with close database architecture calls that is not with ExecuteReader and ExecuteXMLReader also only when Transaction is not required.
        /// </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataBaseManagerOracle(IDBContextOracle context, QuerySpecification querySpecification, bool autoDisposeConnection = true)
        {
            _querySpecification = querySpecification; _autoDisposeConnection = autoDisposeConnection;
            _conn = (_context = context).GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "");
        }
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.DatabaseManagers.DataBaseManagerOracle class.
        /// </summary>
        /// <param name="context">               (Immutable) the context. </param>
        /// <param name="connectionString">      The connection string. </param>
        /// <param name="autoDisposeConnection">
        /// (Optional)
        /// (Immutable) true to automatically dispose connection. default is false.
        ///  Auto disposition of connections will work only with close database architecture calls that is not with ExecuteReader and ExecuteXMLReader also only when Transaction is not required.
        /// </param>
        public DataBaseManagerOracle(IDBContextOracle context, string connectionString, bool autoDisposeConnection = false)
        {
            _autoDisposeConnection = autoDisposeConnection;
            _conn = (_context = context).GetConnection(connectionString + "");
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleCommand CreateCommand(string query, CommandType type, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            OracleCommand cmd = new(query, _conn);
            if (Trans is not null) cmd.Transaction = Trans;
            cmd.BindByName = true;
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
                                if (isMapUDTAsJSON || isMapUDTAsXML)
                                {
                                    OracleClob clob = new(_conn);
                                    var arr = Encoding.Unicode.GetBytes(item.Value as string);
                                    clob.Write(arr, 0, arr.Length);
                                    cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(":") ? item.Name.Replace(":", "") : item.Name, OracleDbType.Clob, clob, ParameterDirection.Input);
                                }
                                else
                                {
                                    var p = cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(":") ? CommandType.Text == type ? item.Name : ":" + item.Name : CommandType.StoredProcedure == type ? item.Name.Replace(":", "") : item.Name, OracleDbType.Object, item.Value, ParameterDirection.Input);
                                    p.UdtTypeName = item.Tag;
                                }
                                break;
                            }
                        case DatabaseParameterType.Binary when !item.IsOutParameter:
                            {
                                OracleBlob blob = new(_conn);
                                var arr = Encoding.Unicode.GetBytes(item.Value as string);
                                blob.Write(arr, 0, arr.Length);
                                cmd.Parameters.Add(!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(":") ? item.Name.Replace(":", "") : item.Name, OracleDbType.Blob, blob, ParameterDirection.Input);
                                break;
                            }
                        default:
                            {
                                var p = cmd.Parameters.Add(new OracleParameter());
                                if (item.IsOutParameter)
                                {
                                    p.Direction = ParameterDirection.Output;
                                    p.Size = item.Size is > 0 ? item.Size : p.Size;
                                    if (item.Type == DatabaseParameterType.RefCursor)
                                        p.OracleDbType = OracleDbType.RefCursor;
                                }
                                else
                                    p.Value = item.Value; p.Direction = ParameterDirection.Input;
                                p.ParameterName = !string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(":") ? CommandType.Text == type ? item.Name : ":" + item.Name : CommandType.StoredProcedure == type ? item.Name.Replace(":", "") : item.Name;
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQuery(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalar(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReader(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public OracleDataReader ExecDataReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReader(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataSet ExecDataSet(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTable(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProc(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable ExecDataTableProcWithCommandOut(string query, List<DatabaseParameters> parameters, bool isMapUDTAsJSON, bool isMapUDTAsXML, out OracleCommand command)
        {
            try
            {
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQuery(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQuery(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
        /// <summary>
        ///     Executes the 'non query with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public int ExecNonQueryWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecNonQueryWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public int ExecNonQueryProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecNonQueryProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public int ExecNonQueryProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecNonQueryProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public object ExecScalar(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalar(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public object ExecScalarWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecScalarWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
        /// <summary>
        ///     Executes the 'scalar procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecScalarProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
        /// <summary>
        ///     Executes the 'scalar procedure with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object ExecScalarProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecScalarProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public OracleDataReader ExecDataReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReader(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public OracleDataReader ExecDataReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataReaderWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public OracleDataReader ExecDataReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataReaderProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public OracleDataReader ExecDataReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataReaderProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReader(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReader(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
        /// <summary>
        ///     Executes the 'xml reader with command out' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <param name="command">            [out] The command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecXmlReaderWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
        /// <summary>
        ///     Executes the 'xml reader procedure' operation.
        /// </summary>
        /// <param name="querySpecification"> (Immutable) the query specification. </param>
        /// <param name="parameters">         Options for controlling the operation. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecXmlReaderProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public XmlReader ExecXmlReaderProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecXmlReaderProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public DataSet ExecDataSet(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataSet(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public DataSet ExecDataSetWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataSetWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public DataSet ExecDataSetProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataSetProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public DataSet ExecDataSetProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataSetProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public DataTable ExecDataTable(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataTable(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public DataTable ExecDataTableWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataTableWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        public DataTable ExecDataTableProc(QuerySpecification querySpecification, List<DatabaseParameters> parameters) => ExecDataTableProc(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML);
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
        public DataTable ExecDataTableProcWithCommandOut(QuerySpecification querySpecification, List<DatabaseParameters> parameters, out OracleCommand command) => ExecDataTableProcWithCommandOut(querySpecification.Query, parameters, querySpecification.IsMapUDTAsJSON, querySpecification.IsMapUDTAsXML, out command);
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
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
                    if (_conn.State == ConnectionState.Open) _conn.Close();
                    _conn.Dispose();
                    _conn = null;
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
                Trans = null;
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
                Trans = null;
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
            Rollback();
            Trans = _conn?.BeginTransaction(IsolationLevel.ReadUncommitted);
            return Transaction;
        }
    }
}