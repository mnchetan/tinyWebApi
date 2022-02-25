// <copyright file="DBContextSql.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Helpers;
using tiny.WebApi.IDBContext;
namespace tiny.WebApi.DBContext
{
    /// <summary>
    ///     A database context sql.
    /// </summary>
    /// <seealso cref="T:tiny.WebApi.IDBContext.IDBContextSql"/>
    [DebuggerStepThrough]
    public class DBContextSql : IDBContextSql
    {
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.ExecuteDataReader(SqlCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlDataReader ExecuteDataReader(SqlCommand sqlCommand)
        {
            try
            {
                Global.LogDebug("Inside ExecuteDataReader, get connection and execute.");
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    return ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteReader(); }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    return sqlCommand.ExecuteReader();
                }
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
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.ExecuteNonQuery(SqlCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public int ExecuteNonQuery(SqlCommand sqlCommand)
        {
            try
            {
                Global.LogDebug("Inside ExecuteNonQuery, get connection and execute.");
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    return ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteNonQuery(); }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    return sqlCommand.ExecuteNonQuery();
                }
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
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.ExecuteScalar(SqlCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public object ExecuteScalar(SqlCommand sqlCommand)
        {
            try
            {
                Global.LogDebug("Inside ExecuteScalar, get connection and execute.");
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    return ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteScalar(); }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    return sqlCommand.ExecuteScalar();
                }
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
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.ExecuteXmlReader(SqlCommand)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public XmlReader ExecuteXmlReader(SqlCommand sqlCommand)
        {
            try
            {
                Global.LogDebug("Inside ExecuteXmlReader, get connection and execute.");
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    return ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteXmlReader(); }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    return sqlCommand.ExecuteXmlReader();
                }
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
        /// <param name="sqlDataAdapter"> The SQL data adapter. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.FillDataSet(SqlDataAdapter)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet FillDataSet(SqlDataAdapter sqlDataAdapter)
        {
            try
            {
                Global.LogDebug("Inside FillDataSet, get connection and fill.");
                DataSet ds = new();
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    ds = ImpersonationHelper.Execute(() =>
                    {
                        sqlDataAdapter.SelectCommand.Connection.Open();
                        sqlDataAdapter.Fill(ds);
                        return ds;
                    }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    sqlDataAdapter.Fill(ds);
                }
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
        /// <param name="sqlDataAdapter"> The SQL data adapter. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.FillDataTable(SqlDataAdapter)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataTable FillDataTable(SqlDataAdapter sqlDataAdapter)
        {
            try
            {
                Global.LogDebug("Inside FillDataTable, get connection and fill."); 
                DataTable dt = new();
                if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
                {
                    dt = ImpersonationHelper.Execute(() =>
                    {
                        sqlDataAdapter.SelectCommand.Connection.Open();
                        sqlDataAdapter.Fill(dt);
                        return dt;
                    }, _querySpecification.DatabaseSpecification);
                }
                else
                {
                    GetConnection(_connectionString, true);
                    sqlDataAdapter.Fill(dt);
                }
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
        /// <seealso cref="M:IDBContextSql.GetConnection(string)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlConnection GetConnection(string connectionString, bool isOpenConnection = false)
        {
            Global.LogDebug("Inside GetConnection, create new connection object.");
            _connectionString = connectionString + "";
            if (_connection == null || (_connection is not null && string.IsNullOrWhiteSpace(_connection.ConnectionString)))
                _connection = new SqlConnection(_connectionString);
            Global.LogDebug("If connection state is not open and isOpenConnection as true and impersonation is not needed then open the connection.");
            if (_querySpecification is not null && _querySpecification.DatabaseSpecification is not null && !_querySpecification.DatabaseSpecification.IsImpersonationNeeded && _connection.State != ConnectionState.Open && isOpenConnection) _connection.Open();
            Global.LogDebug("Return connection.");
            return _connection;
        }
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="isOpenConnection"> The isOpenConnection = false by default. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.GetConnection(QuerySpecification)"/>
        SqlConnection IDBContextSql.GetConnection(QuerySpecification querySpecification, bool isOpenConnection)
        {
            _querySpecification = querySpecification;
            return GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(_querySpecification.DatabaseSpecification.ConnectionString + "", _querySpecification.DatabaseSpecification.EncryptionKey) : _querySpecification.DatabaseSpecification.ConnectionString + "", isOpenConnection);
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private QuerySpecification _querySpecification;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Auto Dispose Connection.
        /// </summary>
        [DebuggerHidden]
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// SQL Transaction.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SqlTransaction Transaction { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString = "";
        /// <summary>
        /// SQL Connection
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SqlConnection _connection;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        protected virtual void Dispose(bool disposing)
        {
            Global.LogDebug("Inside Dispose, If not already disposed.");
            if (!_disposed)
            {
                Rollback();
                Global.LogDebug("When disposing is true and connection is not null.");
                if (disposing && _connection is not null)
                {
                    Global.LogDebug("Lock when disposing connection.");
                    lock (_lockObject)
                    {
                        Global.LogDebug("Close connection when open, dispose and set as null.");
                        if (_connection.State == ConnectionState.Open) _connection.Close();
                        _connection.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        _connection = null;
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
        [DebuggerStepThrough]
        [DebuggerHidden]
        public void Rollback()
        {
            Global.LogDebug("Inside rollback, rollback if transaction is not null.");
            if (Transaction is not null)
            {
                Global.LogDebug("Rolling back transaction.");
                Transaction.Rollback();
                Global.LogDebug("Transaction rolled back.");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                Transaction = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
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
            Global.LogDebug("Inside Dispose.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
