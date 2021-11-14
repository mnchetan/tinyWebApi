/// <copyright file="Base.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.Exceptions;
using tinyWebApi.Common.IDataContracts;
using tinyWebApi.Common.IDBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using tinyWebApi.Common.Helpers;
namespace tinyWebApi.Common.Controllers
{
    /// <summary>
    /// The base.
    /// </summary>
    [DebuggerStepThrough]
    public class Base : ControllerBase, IBase
    {
        /// <summary>
        ///     (Immutable) the.
        /// </summary>
        private readonly IBase _;
        /// <summary>
        ///     (Immutable) the logger.
        /// </summary>
        private readonly ILogger<Base> _logger;
        /// <summary>
        ///     Gets or sets the type of the out put.
        /// </summary>
        /// <seealso cref="P:tinyWebApi.Common.IDataContracts.IBase.OutPutType"/>
        [DebuggerHidden]
        OutPutType IBase.OutPutType { get; set; } = OutPutType.JSON;
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.Helpers.Base class.
        /// </summary>
        /// <param name="logger">        (Immutable) the logger. </param>
        /// <param name="sqlContext">    (Immutable) context for the SQL. </param>
        /// <param name="oracleContext"> (Immutable) context for the oracle. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public Base(ILogger<Base> logger, IDBContextSql sqlContext, IDBContextOracle oracleContext)
        {
            _ = this; _logger = logger;
        }
        /// <summary>
        ///     Executes the 'asynchronous' operation.
        /// </summary>
        /// <exception cref="CustomException"> Thrown when a Custom error condition occurs. </exception>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="f">               A Func&lt;T&gt; to process. </param>
        /// <param name="cancellation">    A token that allows processing to be cancelled. </param>
        /// <param name="callerName">      (Optional) Name of the caller. </param>
        /// <param name="callerFilePath">  (Optional) Full pathname of the caller file. </param>
        /// <param name="callerLineNumer"> (Optional) The caller line numer. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        /// <seealso cref="M:IBase.ExecuteAsync{T}(Func{T},CancellationToken,string,string,int)"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public async Task<T> ExecuteAsync<T>(Func<T> f, CancellationToken cancellation, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumer = 0)
        {
            try
            {
                SetGlobal();
                _logger.LogInformation($"Starting invocation of method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
                return await Task.Run(f, cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occured while invoking  method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
                throw new CustomException((int)HttpStatusCode.InternalServerError, $"Internal Server Error!!!", ex.Message + "");
            }
            finally
            {
                _logger.LogInformation($"Finishing invocation of method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
            }
        }
        /// <summary>
        ///     Executes the 'asynchronous' operation.
        /// </summary>
        /// <exception cref="CustomException"> Thrown when a Custom error condition occurs. </exception>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="a">               An Action to process. </param>
        /// <param name="cancellation">    A token that allows processing to be cancelled. </param>
        /// <param name="callerName">      (Optional) Name of the caller. </param>
        /// <param name="callerFilePath">  (Optional) Full pathname of the caller file. </param>
        /// <param name="callerLineNumer"> (Optional) The caller line numer. </param>
        /// <seealso cref="M:IBase.ExecuteAsync{T}(Action,CancellationToken,string,string,int)"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public async void ExecuteAsync<T>(Action a, CancellationToken cancellation, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumer = 0)
        {
            try
            {
                SetGlobal();
                _logger.LogInformation($"Starting invocation of method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
                await Task.Run(a, cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occured while invoking  method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
                throw new CustomException((int)HttpStatusCode.InternalServerError, $"Internal Server Error!!!", ex.Message + "");
            }
            finally
            {
                _logger.LogInformation($"Finishing invocation of method name : {callerName}, file path : {callerFilePath}, line number : {callerLineNumer}, DateTime : {DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}");
            }
        }
        /// <summary>
        /// Sets the global.
        /// </summary>
        private void SetGlobal()
        {
            Global.ServicePort = HttpContext.Connection.LocalPort;
            Global.ServiceIP = HttpContext.Connection.LocalIpAddress;
            Global.CurrentHttpContext = HttpContext;
        }
        /// <summary>
        ///     Gets request specification.
        /// </summary>
        /// <exception cref="CustomException"> Thrown when a Custom error condition occurs. </exception>
        /// <param name="request">              The request. </param>
        /// <param name="executionType">        Type of the execution. </param>
        /// <param name="outPutType">           Type of the out put. </param>
        /// <param name="hasFileContent">       True if has file content, false if not. </param>
        /// <param name="fileContentType">      Type of the file content. </param>
        /// <param name="fileContentFieldName"> Name of the file content field (For multiple files it should be comma seperated but should of of same type). </param>
        /// <param name="sheetName">            Name of the sheet. </param>
        /// <returns>
        ///     The request specification.
        /// </returns>
        /// <seealso cref="M:IBase.GetRequestSpecification(dynamic,ExecutionType,OutPutType,bool,FileContentType,string,string)"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public List<RequestSpecification> GetRequestSpecification(dynamic request, ExecutionType executionType, OutPutType outPutType, bool hasFileContent, FileContentType fileContentType, string fileContentFieldName, string sheetName)
        {
            try
            {
                if (hasFileContent && executionType != ExecutionType.DataSetProcedure && executionType != ExecutionType.DataTableProcedure)
                    throw new CustomException((int)HttpStatusCode.InternalServerError, "Internal Server Error!!!", "Request having file content should have execution type either DataTableProcedure or DataSetProcedure.");
                if ((executionType == ExecutionType.NonQueryProcedure || executionType == ExecutionType.NonQueryText || executionType == ExecutionType.ScalarProcedure || executionType == ExecutionType.ScalarText) && outPutType != OutPutType.JSON)
                    throw new CustomException((int)HttpStatusCode.InternalServerError, "Internal Server Error!!!", "Request execution type ScalarText, ScalarProcedure, NonQueryText, NonQueryProcedure can only support JSON serialized output.");
                var result = new List<RequestSpecification>();
                foreach (var (p, r) in from KeyValuePair<string, JToken> p in request as JObject
                                       let r = new RequestSpecification
                                       {
                                           IsArrayOfSingleField = p.Value.HasValues,
                                           PropertyName = p.Key,
                                           CallType = "P",
                                           PropertyType = null,
                                           IsArrayOfMultipleFields = hasFileContent ? !$"{fileContentFieldName}".Contains($"{p.Key}", StringComparison.OrdinalIgnoreCase) && p.Value.HasValues && p.Value is not null && p.Value.First is not null && p.Value.First.HasValues : p.Value.HasValues && p.Value is not null && p.Value.First.HasValues,
                                           IsFileContent = hasFileContent && $"{fileContentFieldName}".Contains($"{p.Key}", StringComparison.OrdinalIgnoreCase),
                                           FileContentType = fileContentType
                                       }
                                       select (p, r))
                {
                    r.PropertyType = hasFileContent && $"{fileContentFieldName}".Contains($"{p.Key}", StringComparison.OrdinalIgnoreCase) ? null : GetPropertyType(p, r);
                    try
                    {
                        r.PropertyValue = hasFileContent && $"{fileContentFieldName}".Contains($"{p.Key}", StringComparison.OrdinalIgnoreCase) ? fileContentType == FileContentType.CSV ? ExcelCSVHelper.ImportCSVToDataTable(p.Value.ToObject<byte[]>()) : fileContentType == FileContentType.Excel ? ExcelCSVHelper.ImportExcelToDataTable(p.Value.ToObject<byte[]>(), sheetName) : p.Value.ToObject<byte[]>() : r.IsArrayOfSingleField && !r.IsArrayOfMultipleFields ? (dynamic)p.Value.ToObject<List<dynamic>>() : r.IsArrayOfSingleField && r.IsArrayOfMultipleFields ? (dynamic)p.Value : (dynamic)p.Value.ToObject(r.PropertyType);
                    }
                    catch
                    {
                        r.PropertyType = null; r.PropertyValue = null;
                    }
                    result.Add(r);
                }
                var q = _.GetRequestSpecificationFromQueryParameters();
                if (q.Count > 0) result.AddRange(q);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid request!!!");
                throw new CustomException((int)HttpStatusCode.InternalServerError, $"Internal Server Error!!!", $"Invalid request...{Environment.NewLine}Error:{ex.Message + ""}");
            }
        }
        /// <summary>
        ///     Gets request specification from query parameters.
        /// </summary>
        /// <returns>
        ///     The request specification from query parameters.
        /// </returns>
        /// <seealso cref="M:IBase.GetRequestSpecificationFromQueryParameters()"/>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public List<RequestSpecification> GetRequestSpecificationFromQueryParameters() => (from c in HttpContext.Request.Query select new RequestSpecification() { PropertyName = c.Key, PropertyType = null, PropertyValue = c.Value, CallType = "G" }).ToList();
        /// <summary>
        ///     Gets property type.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="r">        A RequestSpecification to process. </param>
        /// <returns>
        ///     The property type.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static Type GetPropertyType(KeyValuePair<string, JToken> property, RequestSpecification r) => r.PropertyType = !r.IsArrayOfMultipleFields ? (r.IsArrayOfSingleField && !r.IsArrayOfMultipleFields ? property.Value.First?.Type : property.Value.Type) switch { JTokenType.Raw or JTokenType.String or JTokenType.TimeSpan or JTokenType.Guid or JTokenType.Uri => typeof(String), JTokenType.Boolean => typeof(Boolean), JTokenType.Date => typeof(DateTime), JTokenType.Integer => typeof(Int64), JTokenType.Float => typeof(Decimal), _ => r.CallType == "P" ? null : typeof(Object), } : typeof(DataTable);
        /// <summary>
        ///     Map out put type.
        /// </summary>
        /// <param name="outPutType">    Type of the out put. </param>
        /// <param name="executionType"> Type of the execution. </param>
        /// <returns>
        ///     An IBase.
        /// </returns>
        /// <seealso cref="M:IBase.MapOutPutType(OutPutType,ExecutionType)"/>
        public IBase MapOutPutType(OutPutType outPutType, ExecutionType executionType)
        {
            _.OutPutType = (executionType == ExecutionType.DataSetProcedure || executionType == ExecutionType.DataSetText || executionType == ExecutionType.DataTableProcedure || executionType == ExecutionType.DataTableText) && outPutType == OutPutType.Excel ? OutPutType.Excel : outPutType == OutPutType.PDF ? OutPutType.PDF : (executionType == ExecutionType.DataTableProcedure || executionType == ExecutionType.DataTableText) && outPutType == OutPutType.CSV ? OutPutType.CSV : OutPutType.JSON;
            return _;
        }
    }
}
