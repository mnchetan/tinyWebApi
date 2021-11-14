/// <copyright file="IDBContextSql.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
namespace tinyWebApi.Common.IDBContext
{
    /// <summary>
    ///     Interface for idb context sql.
    /// </summary>
    public interface IDBContextSql
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
        internal SqlConnection GetConnection(QuerySpecification querySpecification);
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        SqlConnection GetConnection(string connectionString);
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
    }
}