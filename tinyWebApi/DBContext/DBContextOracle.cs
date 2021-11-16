// <copyright file="DBContextOracle.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Diagnostics;
using System.Xml;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.IDBContext;
namespace tinyWebApi.Common.DBContext
{
    /// <summary>
    ///     A database context oracle.
    /// </summary>
    /// <seealso cref="T:tinyWebApi.Common.IDBContext.IDBContextOracle"/>
    [DebuggerStepThrough]
    public class DBContextOracle : IDBContextOracle
    {
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.ExecuteDataReader(OracleCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleDataReader ExecuteDataReader(OracleCommand OracleCommand)
        {
            try
            {
                Global.LogInformation("Inside ExecuteDataReader, get connection and execute.");
                GetConnection(_connectionString, true);
                return OracleCommand.ExecuteReader();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'non query' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.ExecuteNonQuery(OracleCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecuteNonQuery(OracleCommand OracleCommand)
        {
            try
            {
                Global.LogInformation("Inside ExecuteNonQuery, get connection and execute.");
                GetConnection(_connectionString, true);
                return OracleCommand.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.ExecuteScalar(OracleCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecuteScalar(OracleCommand OracleCommand)
        {
            try
            {
                Global.LogInformation("Inside ExecuteScalar, get connection and execute.");
                GetConnection(_connectionString, true);
                return OracleCommand.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.ExecuteXmlReader(OracleCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecuteXmlReader(OracleCommand OracleCommand)
        {
            try
            {
                Global.LogInformation("Inside ExecuteXmlReader, get connection and execute.");
                GetConnection(_connectionString, true);
                return OracleCommand.ExecuteXmlReader();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Fill data set.
        /// </summary>
        /// <remarks>
        /// <param name="OracleDataAdapter"> The oracle data adapter. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.FillDataSet(OracleDataAdapter)"/>
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet FillDataSet(OracleDataAdapter OracleDataAdapter)
        {
            try
            {
                Global.LogInformation("Inside FillDataSet, get connection and fill.");
                GetConnection(_connectionString, true);
                DataSet ds = new();
                OracleDataAdapter.Fill(ds);
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Fill data table.
        /// </summary>
        /// <param name="OracleDataAdapter"> The oracle data adapter. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.FillDataTable(OracleDataAdapter)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable FillDataTable(OracleDataAdapter OracleDataAdapter)
        {
            try
            {
                Global.LogInformation("Inside FillDataTable, get connection and fill.");
                GetConnection(_connectionString, true);
                DataTable dt = new();
                OracleDataAdapter.Fill(dt);
                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Transaction is null && AutoDisposeConnection) Dispose(true);
            }
        }
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <param name="isOpenConnection"> The isOpenConnection = false by default. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.GetConnection(string)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleConnection GetConnection(string connectionString, bool isOpenConnection = false)
        {
            Global.LogInformation("Inside GetConnection, create new connection object.");
            if (_connection == null)
                _connection = new OracleConnection(_connectionString = connectionString);
            Global.LogInformation("If connection state is not open and isOpenConnection as true then open the connection.");
            if (_connection.State != ConnectionState.Open && isOpenConnection) _connection.Open();
            Global.LogInformation("Return connection.");
            return _connection;
        }
        /// <summary>
        /// Auto Dispose Connection.
        /// </summary>
        [DebuggerHidden]
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// Oracle Transaction.
        /// </summary>
        [DebuggerHidden]
        public OracleTransaction Transaction { get; set; }
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// Oracle Connection
        /// </summary>
        private OracleConnection _connection;
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void Dispose(bool disposing)
        {
            Global.LogInformation("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
                Rollback();
                Global.LogInformation("When disposing is true and connection is not null.");
                if (disposing && _connection is not null)
                {
                    Global.LogInformation("Lock when disposing connection.");
                    lock (_lockObject)
                    {
                        Global.LogInformation("Close connection when open, dispose and set as null.");
                        if (_connection.State == ConnectionState.Open) _connection.Close();
                        _connection.Dispose();
                        _connection = null;
                    }
                    Global.LogInformation("Releasing lock.");
                }
                _disposed = true;
            }
        }
        /// <summary>
        ///     Rollbacks this object.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void Rollback()
        {
            Global.LogInformation("Inside rollback, rollback if transaction is not null.");
            if (Transaction is not null)
            {
                Global.LogInformation("Rolling back transaction.");
                Transaction.Rollback();
                Global.LogInformation("Transaction rolled back.");
                Transaction = null;
            }
        }
        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool _disposed;
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:IDisposable.Dispose()"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void Dispose()
        {
            Global.LogInformation("Inside Dispose.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
