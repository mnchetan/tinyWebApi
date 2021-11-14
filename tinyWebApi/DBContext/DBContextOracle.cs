/// <copyright file="DBContextOracle.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.IDBContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using System.Xml;
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
        public OracleDataReader ExecuteDataReader(OracleCommand OracleCommand) => OracleCommand.ExecuteReader();
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
        public int ExecuteNonQuery(OracleCommand OracleCommand) => OracleCommand.ExecuteNonQuery();
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
        public object ExecuteScalar(OracleCommand OracleCommand) => OracleCommand.ExecuteScalar();
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
        public XmlReader ExecuteXmlReader(OracleCommand OracleCommand) => OracleCommand.ExecuteXmlReader();
        /// <summary>
        ///     Fill data set.
        /// </summary>
        /// <remarks>
        /// <param name="OracleDataAdapter"> The oracle data adapter. </param>
        /// <returns>
        ///     A DataSet.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.FillDataSet(OracleDataAdapter)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public DataSet FillDataSet(OracleDataAdapter OracleDataAdapter)
        {
            DataSet ds = new();
            OracleDataAdapter.Fill(ds);
            return ds;
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
            DataTable dt = new();
            OracleDataAdapter.Fill(dt);
            return dt;
        }
        /// <summary>
        ///     Gets a connection.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        /// <returns>
        ///     The connection.
        /// </returns>
        /// <seealso cref="M:IDBContextOracle.GetConnection(string)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OracleConnection GetConnection(string connectionString)
        {
            var con = new OracleConnection(connectionString);
            if (con.State != ConnectionState.Open) con.Open();
            return con;
        }
    }
}
