// <copyright file="BaseRepository.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the base repository class.
// </summary>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.Extensions;
using tinyWebApi.Common.Helpers;
using tinyWebApi.Common.IDBContext;
using ora = tinyWebApi.Common.DatabaseManagers.DataBaseManagerOracle;
using sql = tinyWebApi.Common.DatabaseManagers.DataBaseManagerSql;
namespace tinyWebApi.Common
{
    /// <summary>
    ///     A base repository.
    /// </summary>
    [DebuggerStepThrough]
    public class BaseRepository
    {
        /// <summary>
        ///     (Immutable) context for the SQL.
        /// </summary>
        private readonly IDBContextSql _sqlContext;
        /// <summary>
        ///     (Immutable) context for the oracle.
        /// </summary>
        private readonly IDBContextOracle _oracleContext;
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.BaseRepository class.
        /// </summary>
        /// <param name="sqlContext">    (Immutable) context for the SQL. </param>
        /// <param name="oracleContext"> (Immutable) context for the oracle. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public BaseRepository(IDBContextSql sqlContext, IDBContextOracle oracleContext)
        {
            _sqlContext = sqlContext;
            _oracleContext = oracleContext;
        }
        /// <summary>
        ///     Process the query.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="list">               The list. </param>
        /// <param name="executionType">      Type of the execution. </param>
        /// <returns>
        ///     A string.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string ProcessQuery(QuerySpecification querySpecification, List<DatabaseParameters> list, ExecutionType executionType)
        {
            Global.LogInformation("Inside ProcessQuery, processing the query to process text based queries for SQL and Oracle.");
            switch (executionType)
            {
                case ExecutionType.ScalarText:
                case ExecutionType.NonQueryText:
                case ExecutionType.DataTableText:
                case ExecutionType.DataSetText:
                    {
                        list.ForEach((item) =>
                        {
                            var val = JsonConvert.ToString(item.Value);
                            if (!string.IsNullOrEmpty($"{val}") && val.Length > 0 && !val.Contains(',') && item.Type != DatabaseParameterType.Structured && item.Type != DatabaseParameterType.Binary)
                            {
                                val = val.Replace("'", "''");
                                val = $"'{val}'";
                                val = querySpecification.DatabaseSpecification.DatabaseType == DatabaseType.ORACLE ? val.Replace("&", "' || CHR(38) || '") : val;
                            }
                            querySpecification.Query = querySpecification.DatabaseSpecification.DatabaseType switch
                            {
                                DatabaseType.MSSQL => item.Type != DatabaseParameterType.Structured && item.Type != DatabaseParameterType.Binary && !string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(val) ? querySpecification.Query.Replace($"@{item.Name}", val, StringComparison.OrdinalIgnoreCase) : querySpecification.Query,
                                DatabaseType.ORACLE => item.Type != DatabaseParameterType.Structured && item.Type != DatabaseParameterType.Binary && !string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(val) ? querySpecification.Query.Replace($":{item.Name}", val, StringComparison.OrdinalIgnoreCase) : querySpecification.Query,
                                _ => querySpecification.Query
                            };
                        });
                        _ = querySpecification.DatabaseSpecification.DatabaseType switch
                        {
                            DatabaseType.MSSQL => list.RemoveAll(o => o.Type is not DatabaseParameterType.Structured and not DatabaseParameterType.Binary),
                            DatabaseType.ORACLE => list.RemoveAll(o => o.Type is not DatabaseParameterType.Structured and not DatabaseParameterType.Binary and not DatabaseParameterType.RefCursor),
                            _ => throw new NotImplementedException()
                        };
                        break;
                    }
                default:
                    break;
            }
            return querySpecification.Query;
        }
        /// <summary>
        ///     Process the parameters.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="list">               The list. </param>
        /// <returns>
        ///     A List&lt;DatabaseParameters&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static List<DatabaseParameters> ProcessParameters(QuerySpecification querySpecification, List<DatabaseParameters> list)
        {
            Global.LogInformation("Inside ProcessParameters, processing the process parameters for SQL and Oracle.");
            List<DatabaseParameters> l1 = new();
            switch (querySpecification.DatabaseSpecification.DatabaseType)
            {
                case DatabaseType.MSSQL:
                    var inputParameters = $"{querySpecification.InputFieldNamesInSequence_UDTDollarSeperatedByType}".Split("$");
                    foreach (var (dbp, input) in inputParameters.SelectMany(input => list.Where(dbp => $"{input}".Replace(",", "").Replace("@", "").Split("$")[0].Replace("$", "").ToLower() == $"{dbp.Name}".ToLower()).Select(dbp => (dbp, input))))
                    {
                        if (dbp.Type == DatabaseParameterType.Structured && input.Contains('$'))
                        {
                            var strSplit = $"{input}".Split("$");
                            dbp.Tag = strSplit.Length == 2 ? $"{strSplit[1]}" : "dbo." + input;
                        }
                        l1.Add(dbp);
                    }
                    l1 = CopyUnMappedDatabaseParameters(list, l1);
                    break;
                case DatabaseType.ORACLE:
                    var inputs = $"{querySpecification.InputFieldNamesInSequence_UDTDollarSeperatedByType}".Split("$");
                    var outPutParameters = $"{querySpecification.Outputs_RefCursor_InSequence_CommaSeperated_WithIntFieldsSeperatedByColon_NotRequiredForMSSQL}".Split(',');
                    foreach (var output in outPutParameters)
                    {
                        if (!string.IsNullOrEmpty(output))
                        {
                            DatabaseParameters dbp = new();
                            dbp.IsOutParameter = true;
                            dbp.Name = $"{output}".Contains(":") ? $"{output}".Split(":")[0].Replace(":", "") : $"{output}".Replace(",", "");
                            dbp.Type = DatabaseParameterType.RefCursor;
                            l1.Add(dbp);
                        }
                    }
                    foreach (var (dbp, input) in inputs.SelectMany(input => list.Where(dbp => $"{input}".Replace(",", "").Replace(":", "").Split("$")[0].Replace("$", "").ToLower() == $"{dbp.Name}".ToLower()).Select(dbp => (dbp, input))))
                    {
                        if (dbp.Type == DatabaseParameterType.Structured && input.Contains('$'))
                        {
                            var strSplit = $"{input}".Split("$");
                            if (strSplit.Length == 2) dbp.Tag = $"{strSplit[1]}";
                            l1.Add(dbp);
                        }
                        else if (dbp.Type == DatabaseParameterType.Structured && !input.Contains('$'))
                        {
                            dbp.Value = querySpecification.IsMapUDTAsJSON_ApplicableForOracle && dbp.Value is DataTable ? dbp.Value.ToJSON() : querySpecification.IsMapUDTAsXML_ApplicableForOracle && dbp.Value is DataTable ? (dbp.Value as DataTable).DataTableAsXML() : throw new NotSupportedException("User defined type support if needed to be used then type name should be specified as $ seperated in the inputs within the queryspecification else should be mapped either as xml or json and should be of type a table/array...");
                            l1.Add(dbp);
                        }
                    }
                    l1 = CopyUnMappedDatabaseParameters(list, l1);
                    break;
                default:
                    break;
            }
            return l1;
        }
        /// <summary>
        ///     Copies the un mapped database parameters.
        /// </summary>
        /// <param name="list"> The list. </param>
        /// <param name="l1">   The first List&lt;DatabaseParameters&gt; </param>
        /// <returns>
        ///     A List&lt;DatabaseParameters&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static List<DatabaseParameters> CopyUnMappedDatabaseParameters(List<DatabaseParameters> list, List<DatabaseParameters> l1)
        {
            Global.LogInformation("Inside CopyUnMappedDatabaseParameters, Adding the unmapped database parameters.");
            foreach (var item in list)
            {
                var notFound = true;
                foreach (var _ in l1.Where(inp => inp.Name == item.Name).Select(inp => new { }))
                {
                    notFound = false;
                    break;
                }
                if (notFound) l1.Add(item);
            }
            return l1;
        }
        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <returns>
        ///     The parameters.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static List<DatabaseParameters> GetParameters(List<RequestSpecification> requestSpecifications)
        {
            Global.LogInformation("Inside GetParameters, Getting the database parameters from requestspecification.");
            List<DatabaseParameters> list = new();
            foreach (var (item, l) in from item in requestSpecifications let l = new DatabaseParameters() select (item, l))
            {
                if (item.DatabaseParameterType == DatabaseParameterType.UnKnown) continue;
                l.Name = item.PropertyName;
                l.Type = item.DatabaseParameterType;
                l.Value = item.Value;
                list.Add(l);
            }
            return list;
        }
        /// <summary>
        ///     Executes the 'query' operation.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="executionType">         Type of the execution. </param>
        /// <param name="list">                  The list. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="outPutType">            (Optional) Type of the out put, default JSON. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public virtual dynamic ExecuteQuery(string key, QuerySpecification querySpecification, ExecutionType executionType, List<DatabaseParameters> list, List<RequestSpecification> requestSpecifications, OutPutType outPutType = OutPutType.JSON) => (executionType, querySpecification.DatabaseSpecification.DatabaseType, outPutType) switch
        {
            (ExecutionType.DataSetText, DatabaseType.MSSQL, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.MSSQL, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.MSSQL, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSet(querySpecification, list).Tables[0].AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.MSSQL, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.ORACLE, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.ORACLE, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.ORACLE, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSet(querySpecification, list).Tables[0].AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetText, DatabaseType.ORACLE, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSet(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.MSSQL, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.MSSQL, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.MSSQL, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.MSSQL, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.ORACLE, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.ORACLE, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.ORACLE, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableText, DatabaseType.ORACLE, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTable(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.MSSQL, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.MSSQL, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.MSSQL, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSetProc(querySpecification, list).Tables[0].AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.MSSQL, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.ORACLE, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.ORACLE, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.ORACLE, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSetProc(querySpecification, list).Tables[0].AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataSetProcedure, DatabaseType.ORACLE, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataSet>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataSetProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.MSSQL, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.MSSQL, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.MSSQL, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.MSSQL, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new sql(_sqlContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.ORACLE, OutPutType.Excel) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForExcel(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.ORACLE, OutPutType.PDF) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForPDF(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.ORACLE, OutPutType.CSV) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutDataForCSV(key, querySpecification, requestSpecifications),
            (ExecutionType.DataTableProcedure, DatabaseType.ORACLE, OutPutType.JSON) => querySpecification.CachingEnabled() && querySpecification.AllowServeFromCache(executionType) ? querySpecification.ServeFromCache<DataTable>(executionType).ProcessOutPutData(key, querySpecification, requestSpecifications) : new ora(_oracleContext, querySpecification).ExecDataTableProc(querySpecification, list).AddToCache(querySpecification, executionType).ProcessOutPutData(key, querySpecification, requestSpecifications),
            (ExecutionType.NonQueryText, DatabaseType.MSSQL, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new sql(_sqlContext, querySpecification).ExecNonQuery(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.NonQueryText, DatabaseType.ORACLE, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new ora(_oracleContext, querySpecification).ExecNonQuery(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.ScalarText, DatabaseType.MSSQL, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new sql(_sqlContext, querySpecification).ExecScalar(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.ScalarText, DatabaseType.ORACLE, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new ora(_oracleContext, querySpecification).ExecScalar(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.NonQueryProcedure, DatabaseType.MSSQL, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new sql(_sqlContext, querySpecification).ExecNonQueryProc(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.NonQueryProcedure, DatabaseType.ORACLE, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new ora(_oracleContext, querySpecification).ExecNonQueryProc(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.ScalarProcedure, DatabaseType.MSSQL, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new sql(_sqlContext, querySpecification).ExecScalarProc(querySpecification, list), key, querySpecification, requestSpecifications),
            (ExecutionType.ScalarProcedure, DatabaseType.ORACLE, OutPutType.JSON) => ProcessDataExtensions.ProcessOutPutScalarNonScalar(new ora(_oracleContext, querySpecification).ExecScalarProc(querySpecification, list), key, querySpecification, requestSpecifications),
            _ => throw new NotImplementedException()
        };
    }
}
