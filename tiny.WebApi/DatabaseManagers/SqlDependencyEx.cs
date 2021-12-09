// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
using tiny.WebApi.IDBContext;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Extended Sql Dependency
    /// Note # 1: Service broker account needs to be enabled for the sql database in order to recieve change notifications.
    /// Note # 2: The projected columns in the SELECT statement must be explicitly stated, and table names must be qualified with two-part names.Notice that this means that all tables referenced in the statement must be in the same database.
    /// Note # 3: The statement may not use the asterisk (*) or table_name.* syntax to specify columns.
    /// Note # 4: The statement may not use unnamed columns or duplicate column names.
    /// Note # 5: The statement must reference a base table.
    /// Note # 6: The projected columns in the SELECT statement may not contain aggregate expressions unless the statement uses a GROUP BY expression. When a GROUP BY expression is provided, the select list may contain the aggregate functions COUNT_BIG() or SUM(). However, SUM() may not be specified for a nullable column.The statement may not specify HAVING, CUBE, or ROLLUP.
    /// 
    /// Change Notification permissions Sample : to provide to leats priviledged user:
    /// 
    /// USE YourDatabase;
    /// GO
    /// --create user for schema ownership
    /// CREATE USER SqlDependencySchemaOwner WITHOUT LOGIN;
    /// GO
    /// --create schema for SqlDependency ojbects
    /// CREATE SCHEMA SqlDependency AUTHORIZATION SqlDependencySchemaOwner;
    /// GO
    /// --set the default schema of minimally privileged user to SqlDependency
    /// ALTER USER SqlNotificationUser WITH DEFAULT_SCHEMA = SqlDependency;
    /// --grant user control permissions on SqlDependency schema
    /// GRANT CONTROL ON SCHEMA::SqlDependency TO SqlNotificationUser;
    /// --grant user impersonate permissions on SqlDependency schema owner
    /// GRANT IMPERSONATE ON USER::SqlDependencySchemaOwner TO SqlNotificationUser;
    /// GO
    /// --grant database permissions needed to create and use SqlDependency objects
    /// GRANT CREATE PROCEDURE TO SqlNotificationUser;
    /// GRANT CREATE QUEUE TO SqlNotificationUser;
    /// GRANT CREATE SERVICE TO SqlNotificationUser;
    /// GRANT REFERENCES ON
    /// CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] TO SqlNotificationUser;
    /// GRANT VIEW DEFINITION TO SqlNotificationUser;
    /// GRANT SELECT to SqlNotificationUser;
    /// GRANT SUBSCRIBE QUERY NOTIFICATIONS TO SqlNotificationUser;
    /// GRANT RECEIVE ON QueryNotificationErrorsQueue TO SqlNotificationUser;
    /// GO
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDependencyEx(IDBContextSql context, QuerySpecification querySpecification)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        /// Creates a command of type text.
        /// Note : Make sure only text based queries are used and only number or string type parameters are used whose direction isof type input .
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandTimeOutInSeconds"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private SqlCommand CreateCommand(List<DatabaseParameters> parameters, int commandTimeOutInSeconds)
        {
            Global.LogInformation("Inside CreateCommand.");
            SqlCommand cmd = new(_querySpecification.Query, _conn);
            Global.LogInformation("Setting command type, command timeout.");
            cmd.Notification = null;
            cmd.CommandTimeout = commandTimeOutInSeconds > 0 ? commandTimeOutInSeconds : _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            cmd.CommandType = CommandType.Text;
            Global.LogInformation("Setting command parameters.");
            foreach (var item in parameters) cmd.Parameters.Add(new SqlParameter(item.Name, item.Value));
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
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                    ImpersonationHelper.Execute(() => { ProcessAction(parameters, commandTimeOutInSeconds); }, _querySpecification.DatabaseSpecification);
                else
                    ProcessAction(parameters, commandTimeOutInSeconds);
            }
            catch (Exception ex)
            {
                if (SqlNotificationExceptionEvent is not null)
                    SqlNotificationExceptionEvent(this, new() { Exception = ex });
                Dispose(true);
            }
        }
        /// <summary>
        /// Process Action.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandTimeOutInSeconds"></param>
        /// <returns></returns>
        private int ProcessAction(List<DatabaseParameters> parameters, int commandTimeOutInSeconds = 0)
        {
            var cmd = CreateCommand(parameters, commandTimeOutInSeconds);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            if (ObjWatcher is null)
                ObjWatcher = new(cmd, null, commandTimeOutInSeconds);
            ObjWatcher.OnChange -= ObjWatcher_OnChange;
            ObjWatcher.OnChange += ObjWatcher_OnChange;
            _context.AutoDisposeConnection = false;
            _context.ExecuteNonQuery(cmd);
            return 0;
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
                SqlNotificationEventEx(this, new SqlNotificationEventArgsEx(eventArgs, SharedObject, Guid));
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
        /// Extended Sql Notification event.
        /// </summary>
        public event SqlNotificationHandlerEx? SqlNotificationEventEx;
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
        /// Extended Sql Notification Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SqlNotificationHandlerEx(SqlDependencyEx sender, SqlNotificationEventArgsEx e);
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
        /// Sql notification exception handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SqlNotificationExceptionHandler(SqlDependencyEx sender, ExceptionEventArgs e);
        /// <summary>
        /// Sql notification exception event.
        /// </summary>
        public event SqlNotificationExceptionHandler SqlNotificationExceptionEvent;
    }
    /// <summary>
    /// Extended Error Event Args to return shared object. 
    /// </summary>
    [DebuggerStepThrough]
    public class SqlNotificationEventArgsEx
    {
        /// <summary>
        /// Sql Notification Event Args constructor
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sharedObject"></param>
        /// <param name="guid"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
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
    /// <summary>
    /// Exception event arguments
    /// </summary>
    [DebuggerStepThrough]
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Exception object
        /// </summary>
        [DebuggerHidden]
        public Exception? Exception { get; set; }
    }
}
