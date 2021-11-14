/// <copyright file="IDBContextOracle.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Diagnostics;
using System.Xml;
namespace tinyWebApi.Common.IDBContext
{
    /// <summary>
    ///     Interface for idb context oracle.
    /// </summary>
    public interface IDBContextOracle : IDisposable
    {
        /// <summary>
        ///     Executes the 'non query' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        int ExecuteNonQuery(OracleCommand OracleCommand);
        /// <summary>
        ///     Executes the 'scalar' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        object ExecuteScalar(OracleCommand OracleCommand);
        /// <summary>
        ///     Executes the 'data reader' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An OracleDataReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        OracleDataReader ExecuteDataReader(OracleCommand OracleCommand);
        /// <summary>
        ///     Fill data set.
        /// </summary>
        /// <param name="OracleDataAdapter"> The oracle data adapter. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataSet FillDataSet(OracleDataAdapter OracleDataAdapter);
        /// <summary>
        ///     Fill data table.
        /// </summary>
        /// <param name="OracleDataAdapter"> The oracle data adapter. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataTable FillDataTable(OracleDataAdapter OracleDataAdapter);
        /// <summary>
        ///     Executes the 'xml reader' operation.
        /// </summary>
        /// <param name="OracleCommand"> The oracle command. </param>
        /// <returns>
        ///     An XmlReader.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        XmlReader ExecuteXmlReader(OracleCommand OracleCommand);
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        OracleConnection GetConnection(string connectionString, bool isOpenConnection = false);
        /// <summary>
        /// Auto Dispose Connection.
        /// </summary>
        [DebuggerHidden]
        bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// Oracle Transaction.
        /// </summary>
        [DebuggerHidden]
        OracleTransaction Transaction { get; set; }
    }
}