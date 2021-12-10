// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Extensions;
using tiny.WebApi.IDBContext;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Sql Poller
    /// Recommended is go for count based query or light weight select queries.
    /// Also only go for select queries.
    /// Only text based queries supported.
    /// </summary>
    [DebuggerStepThrough]
    public class SqlPoller : IDisposable
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
        /// Sql Poller constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="querySpecification"></param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlPoller(IDBContextSql context, QuerySpecification querySpecification)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Global.LogInformation("Inside DataBaseManagerSql and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
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
        /// /// <param name="pollIntervalInSeconds"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public DataTable StartWatching(List<DatabaseParameters> parameters, int commandTimeOutInSeconds = 0, int pollIntervalInSeconds = 60)
        {
            try
            {
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                    return ImpersonationHelper.Execute(() => { return ProcessAction(parameters, commandTimeOutInSeconds, pollIntervalInSeconds); }, _querySpecification.DatabaseSpecification);
                else
                    return ProcessAction(parameters, commandTimeOutInSeconds, pollIntervalInSeconds);
            }
            catch (Exception ex)
            {
                Dispose(true);
                Global.LogDebug($"Some error occured while trying to poll sql: Error: {ex.Message}.", ex.ToJSON());
                throw;
            }
        }
        /// <summary>
        /// Process Action.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandTimeOutInSeconds"></param>
        /// <param name="pollIntervalInSeconds"></param>
        /// <returns></returns>
        private DataTable ProcessAction(List<DatabaseParameters> parameters, int commandTimeOutInSeconds = 0, int pollIntervalInSeconds = 60)
        {
            var cmd = CreateCommand(parameters, commandTimeOutInSeconds);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            bool isExit = false;
            _context.AutoDisposeConnection = false;
            DateTime startTime = DateTime.UtcNow;
            DataTable result = new();
            while (!isExit)
            {
                Task.Run(() =>
                {
                    DataTable dt = _context.FillDataTable(new SqlDataAdapter(cmd));
                    if (dt.Rows.Count > 0)
                    {
                        isExit = true;
                        result = dt;
                    }
                    if (DateTime.UtcNow.Subtract(startTime).TotalSeconds >= commandTimeOutInSeconds)
                        isExit = true;
                    else
                    {
                        if (!isExit)
                            Task.Delay(pollIntervalInSeconds * 1000).ConfigureAwait(true);
                    }
                });
            }
            Dispose(true);
            return result;
        }
        /// <summary>
        /// Dispose the File System Watcher Extended object.
        /// </summary>
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
    }
}
