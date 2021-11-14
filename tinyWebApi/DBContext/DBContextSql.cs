/// <copyright file="DBContextSql.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Helpers;
using tinyWebApi.Common.IDBContext;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
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
        public SqlDataReader ExecuteDataReader(SqlCommand sqlCommand) => _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded ? ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteReader(); }, _querySpecification.DatabaseSpecification) : sqlCommand.ExecuteReader();
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
        public int ExecuteNonQuery(SqlCommand sqlCommand) => _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded ? ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteNonQuery(); }, _querySpecification.DatabaseSpecification) : sqlCommand.ExecuteNonQuery();
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
        public object ExecuteScalar(SqlCommand sqlCommand) => _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded ? ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteScalar(); }, _querySpecification.DatabaseSpecification) : sqlCommand.ExecuteScalar();
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
        public XmlReader ExecuteXmlReader(SqlCommand sqlCommand) => _querySpecification.DatabaseSpecification is not null && _querySpecification.DatabaseSpecification.IsImpersonationNeeded ? ImpersonationHelper.Execute(() => { sqlCommand.Connection.Open(); return sqlCommand.ExecuteXmlReader(); }, _querySpecification.DatabaseSpecification) : sqlCommand.ExecuteXmlReader();
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
            sqlDataAdapter.Fill(ds);
            return ds;
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
            sqlDataAdapter.Fill(dt);
            return dt;
        }
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.GetConnection(string)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public SqlConnection GetConnection(string connectionString)
        {
            var con = new SqlConnection(connectionString + "");
            if (!_querySpecification.DatabaseSpecification.IsImpersonationNeeded) con.Open();
            return con;
        }
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        /// <seealso cref="M:IDBContextSql.GetConnection(QuerySpecification)"/>
        SqlConnection IDBContextSql.GetConnection(QuerySpecification querySpecification)
        {
            _querySpecification = querySpecification;
            return GetConnection(querySpecification.DatabaseSpecification.IsEncrypted ? EncryptFactory.Decrypt(_querySpecification.DatabaseSpecification.ConnectionString + "", _querySpecification.DatabaseSpecification.EncryptionKey) : _querySpecification.DatabaseSpecification.ConnectionString + "");
        }

        private QuerySpecification _querySpecification;
    }
}
