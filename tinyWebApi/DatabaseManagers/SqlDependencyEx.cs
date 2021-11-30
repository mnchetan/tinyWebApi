// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Helpers;
using tinyWebApi.Common.IDBContext;

namespace tinyWebApi.Helpers
{
    /// <summary>
    /// Extended Sql Dependency
    /// </summary>
    [DebuggerStepThrough]
    public class SqlDependencyEx : IDisposable
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
        /// Sql Dependency Extended constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="querySpecification"></param>
        public SqlDependencyEx(IDBContextSql context, QuerySpecification querySpecification)
        {
            Global.LogInformation("Inside DataBaseManagerSql and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        /// Sql Dependency Object
        /// </summary>
        [DebuggerHidden]
        private SqlDependency ObjWatcher { get; set; }
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
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlCommand CreateCommand(string query, List<DatabaseParameters> parameters)
        {
            Global.LogInformation("Inside CreateCommand.");
            SqlCommand cmd = new(query, _conn);
            Global.LogInformation("Setting command type, command timeout.");
            cmd.Notification = null;
            cmd.CommandTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            cmd.CommandType = CommandType.Text;
            Global.LogInformation("Setting command parameters.");
            foreach (var item in parameters) cmd.Parameters.Add(new SqlParameter(item.Name, item.Value));
            cmd.Notification = null;
            return cmd;
        }
        /// <summary>
        /// Starts watching the data
        /// </summary>
        /// <param name="parameters"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void StartWatching(List<DatabaseParameters> parameters)
        {
            try
            {
                var cmd = CreateCommand(_querySpecification.Query, parameters);
                if (ObjWatcher is null)
                    ObjWatcher = new();
                ObjWatcher.OnChange -= ObjWatcher_OnChange;
                ObjWatcher.OnChange += ObjWatcher_OnChange;
                _context.ExecuteNonQuery(cmd);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Recieve change notifications.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        private void ObjWatcher_OnChange(object sender, SqlNotificationEventArgs eventArgs)
        {
            if (SqlNotificationEventEx is not null)
            {
                SqlNotificationEventEx(sender, new SqlNotificationEventArgsEx(eventArgs, SharedObject, Guid));
                if (IsNotifyFirstChangeOnly && _conn is not null)
                    lock (_lockObject)
                    {
                        Global.LogInformation("Close connection when open, dispose and set as null.");
                        if (_conn.State == ConnectionState.Open) _conn.Close();
                        _conn.Dispose();
                        _conn = null;
                    }
            }
        }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        /// <summary>
        /// Extended Sql Notification event.
        /// </summary>
        public event SqlNotificationHandlerEx? SqlNotificationEventEx;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        /// <summary>
        /// Dispose the File System Watcher Extended object.
        /// </summary>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Dispose()
        {
            SharedObject = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Extended Sql Notification Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SqlNotificationHandlerEx(object sender, SqlNotificationEventArgsEx e);
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
                        _conn = null;
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
    }
    /// <summary>
    /// Extended Error Event Args to return shared object. 
    /// </summary>
    public class SqlNotificationEventArgsEx
    {
        /// <summary>
        /// Sql Notification Event Args constructor
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sharedObject"></param>
        /// <param name="guid"></param>
        public SqlNotificationEventArgsEx(SqlNotificationEventArgs e, dynamic sharedObject, Guid guid)
        {
            SqlNotificationEventArgs = e;
            SharedObject = sharedObject;
            Guid = guid;
        }
        /// <summary>
        /// Sql Notification Event Args.
        /// </summary>
        [DebuggerHidden]
        public SqlNotificationEventArgs SqlNotificationEventArgs { get; set; }
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
