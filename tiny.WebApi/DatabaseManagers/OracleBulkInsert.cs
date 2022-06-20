// <copyright file="OracleBulkInsert.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
using tiny.WebApi.IDBContext;
using System.Linq;

namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Oracle Bulk Insert
    /// </summary>
    [DebuggerStepThrough]
    public class OracleBulkInsert : IDisposable
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
        /// Oracle Bulk Copy constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="querySpecification"></param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleBulkInsert(IDBContextOracle context, QuerySpecification querySpecification)
        {
            Global.LogDebug("Inside DataBaseManagerOracle and setting up the parameters.");
            _context = context;
            _querySpecification = querySpecification;
            _conn = _context.GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(querySpecification.DatabaseSpecification.ConnectionString + "", querySpecification?.DatabaseSpecification?.EncryptionKey + "") : querySpecification.DatabaseSpecification.ConnectionString + "", false);
        }
        /// <summary>
        /// Get column mappings.
        /// </summary>
        /// <param name="querySpecification"></param>
        /// <param name="oracleBulkCopy"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void GetColumnMapping(QuerySpecification querySpecification, OracleBulkCopy oracleBulkCopy)
        {
            if (!string.IsNullOrEmpty(querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma))
            {
                querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma = querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace(@"\:", "cnmcolon");
                querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma = querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace(@"\,", "cnmcomma");
                foreach (var j in from i in querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Split(",") let j = i.Replace(",", "").Split(":") select j) oracleBulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping() { SourceColumn = j[0].Replace(":", "").Replace("cnmcolon", ":").Replace("cnmcomma", ","), DestinationColumn = j[1].Replace(":", "").Replace("cnmcolon", ":").Replace("cnmcomma", ",") });
                querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma = querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace("cnmcolon", @"\:");
                querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma = querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace("cnmcomma", @"\,");
            }
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
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();
                using OracleBulkCopy oracleBulkCopy = new(_conn, OracleBulkCopyOptions.UseInternalTransaction);
                GetColumnMapping(_querySpecification, oracleBulkCopy);
                oracleBulkCopy.DestinationTableName = _querySpecification.Query;
                oracleBulkCopy.BulkCopyTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (!string.IsNullOrEmpty(_querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                {
                    var mapping = _querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace(@"\,", ",").Replace(@"\:", ":").ToLower();
                    foreach (var col in from DataColumn col in dt.Columns where !mapping.Contains(col.ColumnName.ToLower()) select col) dt.Columns.Remove(col);
                }
                oracleBulkCopy.WriteToServer(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                Dispose(true);
            }
            return 0;
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
                foreach (var item in requestSpecification)
                {
                    if (item.PropertyType == typeof(DataTable))
                    {
                        if (_conn.State != ConnectionState.Open)
                            _conn.Open();
                        using OracleBulkCopy oracleBulkCopy = new(_conn, OracleBulkCopyOptions.UseInternalTransaction);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        GetColumnMapping(_querySpecification, oracleBulkCopy);
                        oracleBulkCopy.DestinationTableName = item.PropertyName;
                        oracleBulkCopy.BulkCopyTimeout = _querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.ConnectionTimeOut > 0 ? _querySpecification.DatabaseSpecification.ConnectionTimeOut : 1200;
                        using var dt = item.PropertyValue as DataTable;
                        if (!string.IsNullOrEmpty(_querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        {
                            var mapping = _querySpecification.SourceDestinationColumnMapping_SourceDestinationSeperatedbyColonAndRepeatedbyComma.Replace(@"\,", ",").Replace(@"\:", ":").ToLower();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            foreach (var col in from DataColumn col in dt.Columns where !mapping.Contains(col.ColumnName.ToLower()) select col) dt.Columns.Remove(col);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        }
                        oracleBulkCopy.WriteToServer(dt);
                    }
                }
            }
            catch
            {
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
            Global.LogDebug("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
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
        /// Disposed flag
        /// </summary>
        private bool _disposed;
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
