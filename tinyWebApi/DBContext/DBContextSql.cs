/// <copyright file="DBContextSql.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Helpers;
using tinyWebApi.Common.IDBContext;
namespace tinyWebApi.Common.DBContext
{
    /// <summary>
    ///     A database context sql.
    /// </summary>
    /// <seealso cref="T:tinyWebApi.Common.IDBContext.IDBContextSql"/>
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
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
                DataSet ds = new();
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
                DataTable dt = new();
                if (_querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded)
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
            _connectionString = connectionString + "";
            if (_connection == null)
                _connection = new SqlConnection(_connectionString);
            if (!_querySpecification.DatabaseSpecification.IsImpersonationNeeded && isOpenConnection) _connection.Open();
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

        private QuerySpecification _querySpecification;
        /// <summary>
        /// Auto Dispose Connection.
        /// </summary>
        [DebuggerHidden]
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// SQL Transaction.
        /// </summary>
        [DebuggerHidden]
        public SqlTransaction Transaction { get; set; }
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString = "";
        /// <summary>
        /// SQL Connection
        /// </summary>
        private SqlConnection _connection;
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
                if (disposing && _connection is not null)
                {
                    lock (_lockObject)
                    {
                        if (_connection.State == ConnectionState.Open) _connection.Close();
                        _connection.Dispose();
                        _connection = null;
                    }
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
            if (Transaction is not null)
            {
                Transaction.Rollback();
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// (Immutable) Lock Object.
        /// </summary>
        private readonly object _lockObject = new();
    }
}
