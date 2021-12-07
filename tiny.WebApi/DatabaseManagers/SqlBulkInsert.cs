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
    /// Sql Bulk Insert
    /// </summary>
    [DebuggerStepThrough]
    public class SqlBulkInsert : IDisposable
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
        /// Sql Bulk Copy constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="querySpecification"></param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlBulkInsert(IDBContextSql context, QuerySpecification querySpecification)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Global.LogInformation("Inside DataBaseManagerSql and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        /// Process Action.
        /// </summary>
        /// <param name="dt"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private void ProcessAction(DataTable dt)
        {
            using SqlBulkCopy SqlBulkCopy = new(_conn, SqlBulkCopyOptions.UseInternalTransaction, _context.Transaction);
            SqlBulkCopy.DestinationTableName = _querySpecification.Query;
            SqlBulkCopy.BulkCopyTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
            _conn.Open();
            SqlBulkCopy.WriteToServer(dt);
            _context.Transaction?.Commit();
        }
        /// <summary>
        /// Bulk insert data to the destination table.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int BulkInsert(DataTable dt)
        {
            try
            {
                _context.Transaction = _conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded) ImpersonationHelper.Execute(() => ProcessAction(dt), _querySpecification.DatabaseSpecification); else ProcessAction(dt);
            }
            catch
            {
                _context.Transaction?.Rollback();
                throw;
            }
            finally
            {
                Dispose(true);
            }
            return 0;
        }
        /// <summary>
        /// Process the action.
        /// </summary>
        /// <param name="requestSpecification"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private void ProcessAction(List<RequestSpecification> requestSpecification)
        {
            foreach (var item in requestSpecification)
            {
                if (item.PropertyType == typeof(DataTable))
                {
                    using SqlBulkCopy SqlBulkCopy = new(_conn, SqlBulkCopyOptions.UseInternalTransaction, _context.Transaction);
                    SqlBulkCopy.DestinationTableName = item.PropertyName;
                    SqlBulkCopy.BulkCopyTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
                    _conn.Open();
                    SqlBulkCopy.WriteToServer(item.PropertyValue as DataTable);
                }
            }
        }
        /// <summary>
        /// Bulk insert data to the destination table.
        /// </summary>
        /// <param name="requestSpecification"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int BulkInsert(List<RequestSpecification> requestSpecification)
        {
            try
            {
                _context.Transaction = _conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded) ImpersonationHelper.Execute(() => ProcessAction(requestSpecification), _querySpecification.DatabaseSpecification);
                else ProcessAction(requestSpecification);                
                _context.Transaction?.Commit();
            }
            catch
            {
                _context.Transaction?.Rollback();
                throw;
            }
            finally
            {
                Dispose(true);
            }
            return 0;
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
