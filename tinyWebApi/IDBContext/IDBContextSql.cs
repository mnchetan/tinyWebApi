/// <copyright file="IDBContextSql.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using tinyWebApi.Common.DataObjects;
namespace tinyWebApi.Common.IDBContext
{
    /// <summary>
    ///     Interface for idb context sql.
    /// </summary>
    public interface IDBContextSql : IDisposable
    {
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal SqlConnection GetConnection(QuerySpecification querySpecification, bool isOpenConnection = false);
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        SqlConnection GetConnection(string connectionString, bool isOpenConnection = false);
        /// <summary>
        ///     Executes the 'non query' operation.
        /// </summary>
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        int ExecuteNonQuery(SqlCommand sqlCommand);
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        object ExecuteScalar(SqlCommand sqlCommand);
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     A SqlDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        SqlDataReader ExecuteDataReader(SqlCommand sqlCommand);
        /// <summary>
        ///     Fill data set.
        /// </summary>
        /// <param name="sqlDataAdapter"> The SQL data adapter. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataSet FillDataSet(SqlDataAdapter sqlDataAdapter);
        /// <summary>
        ///     Fill data table.
        /// </summary>
        /// <param name="sqlDataAdapter"> The SQL data adapter. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataTable FillDataTable(SqlDataAdapter sqlDataAdapter);
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="sqlCommand"> The SQL command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        XmlReader ExecuteXmlReader(SqlCommand sqlCommand);
        /// <summary>
        /// Auto Dispose Connection.
        /// </summary>
        [DebuggerHidden]
        bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// SQL Transaction.
        /// </summary>
        [DebuggerHidden]
        SqlTransaction Transaction { get; set; }
    }
}