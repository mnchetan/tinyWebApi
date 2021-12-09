// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
using tiny.WebApi.IDBContext;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Extended Oracle Dependency
    /// Ensure proper change notification permissions are granted to user.
    /// 
    /// Command : grant change notification to scott;
    /// </summary>
    [DebuggerStepThrough]
    public class OracleDependencyEx : IDisposable
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
        /// Oracle Dependency Extended constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="querySpecification"></param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDependencyEx(IDBContextOracle context, QuerySpecification querySpecification)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Global.LogInformation("Inside DataBaseManagerOracle and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        /// Oracle Dependency Object
        /// </summary>
        [DebuggerHidden]
        private OracleDependency ObjWatcher { get; set; }
        /// <summary>
        /// Shared Object to be passed on along with generated events.
        /// </summary>
        [DebuggerHidden]
        public object SharedObject { get; set; }
        /// <summary>
        /// Unique identifier to identify the monitoring job.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Automatically disables the raising of events after first change occurs.
        /// </summary>
        [DebuggerHidden]
        public bool IsNotifyFirstChangeOnly { get; set; }
        /// <summary>
        /// Creates a command of type text.
        /// Note : Make sure only text based queries are used and only number or string type parameters are used whose direction isof type input .
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandTimeOutInSeconds"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private OracleCommand CreateCommand(List<DatabaseParameters> parameters, int commandTimeOutInSeconds)
        {
            Global.LogInformation("Inside CreateCommand.");
            OracleCommand cmd = new(_querySpecification.Query, _conn);
            Global.LogInformation("Setting command type, command timeout.");
            cmd.Notification = null;
            cmd.BindByName = true;
            cmd.CommandTimeout = commandTimeOutInSeconds > 0 ? commandTimeOutInSeconds : _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            cmd.CommandType = CommandType.Text;
            cmd.AddRowid = true;
            Global.LogInformation("Setting command parameters.");
            foreach (var item in parameters) cmd.Parameters.Add(new OracleParameter(item.Name, item.Value));
            return cmd;
        }
        /// <summary>
        /// Starts watching the data
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandTimeOutInSeconds"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void StartWatching(List<DatabaseParameters> parameters, int commandTimeOutInSeconds = 0)
        {
            try
            {
                var cmd = CreateCommand(parameters, commandTimeOutInSeconds);
                if (_conn.State != ConnectionState.Open) _conn.Open();
                if (ObjWatcher is null) ObjWatcher = new(cmd, !IsNotifyFirstChangeOnly, commandTimeOutInSeconds, false);
                if (cmd.Notification is not null) cmd.Notification.IsNotifiedOnce = IsNotifyFirstChangeOnly;
                ObjWatcher.OnChange -= ObjWatcher_OnChange;
                ObjWatcher.OnChange += ObjWatcher_OnChange;
                _context.AutoDisposeConnection = false;
                _context.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                if (OracleNotificationExceptionEvent is not null)
                    OracleNotificationExceptionEvent(this, new() { Exception = ex });
                Dispose(true);
            }
        }
        /// <summary>
        /// Recieve change notifications.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            if (OracleNotificationEventEx is not null)
            {
                OracleNotificationEventEx(this, new OracleNotificationEventArgsEx(eventArgs, SharedObject, Guid));
                if (IsNotifyFirstChangeOnly && _conn is not null)
                    lock (_lockObject)
                    {
                        Global.LogInformation("Close connection when open, dispose and set as null.");
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        _conn = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
            }
        }
        /// <summary>
        /// Extended Oracle Notification event.
        /// </summary>
        public event OracleNotificationHandlerEx? OracleNotificationEventEx;
        /// <summary>
        /// Dispose the File System Watcher Extended object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Dispose()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            SharedObject = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Extended Oracle Notification Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OracleNotificationHandlerEx(OracleDependencyEx sender, OracleNotificationEventArgsEx e);
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        protected virtual void Dispose(bool disposing)
        {
            Global.LogInformation("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
                Global.LogInformation("When disposing is true and connection is not null.");
                if (disposing && _conn is not null)
                {
                    Global.LogInformation("Lock when disposing connection.");
                    lock (_lockObject)
                    {
                        Global.LogInformation("Close connection when open, dispose and set as null.");
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        _conn = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                    Global.LogInformation("Releasing lock.");
                }
                _disposed = true;
            }
        }
        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool _disposed;
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
        /// <summary>
        /// Oracle notification exception handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OracleNotificationExceptionHandler(OracleDependencyEx sender, ExceptionEventArgs e);
        /// <summary>
        /// Oracle notification exception event.
        /// </summary>
        public event OracleNotificationExceptionHandler OracleNotificationExceptionEvent;
    }
    /// <summary>
    /// Extended Error Event Args to return shared object. 
    /// </summary>
    [DebuggerStepThrough]
    public class OracleNotificationEventArgsEx
    {
        /// <summary>
        /// Oracle Notification Event Args constructor
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sharedObject"></param>
        /// <param name="guid"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleNotificationEventArgsEx(OracleNotificationEventArgs e, dynamic sharedObject, Guid guid)
        {
            OracleNotificationEventArgs = e;
            SharedObject = sharedObject;
            Guid = guid;
        }
        /// <summary>
        /// Oracle Notification Event Args.
        /// </summary>
        [DebuggerHidden]
        public OracleNotificationEventArgs OracleNotificationEventArgs { get; set; }
        /// <summary>
        /// Shared Objected to be returned back.
        /// </summary>
        [DebuggerHidden]
        public dynamic SharedObject { get; set; }
        /// <summary>
        /// Unique Guid for the call.
        /// </summary>
        [DebuggerHidden]
        public Guid Guid { get; set; }
    }
}
