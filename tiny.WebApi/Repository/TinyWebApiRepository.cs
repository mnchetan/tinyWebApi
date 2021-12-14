// <copyright file="TinyWebApiRepository.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the tiny web API repository class.
// </summary>
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
using tiny.WebApi.Exceptions;
using tiny.WebApi.Extensions;
using tiny.WebApi.IDBContext;
using tiny.WebApi.IRepository;
namespace tiny.WebApi
{
    /// <summary>
    ///     A tiny web API repository.
    /// </summary>
    /// <seealso cref="T:BaseRepository"/>
    /// <seealso cref="T:Itiny.WebApiRepository"/>
    [DebuggerStepThrough]
    public class TinyWebApiRepository : BaseRepository, ITinyWebApiRepository
    {
        /// <summary>
        ///     (Immutable) the logger.
        /// </summary>
        private readonly ILogger<TinyWebApiRepository> _logger;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.tiny.WebApiRepository class.
        /// </summary>
        /// <param name="logger">        (Immutable) the logger. </param>
        /// <param name="sqlContext">    Context for the SQL. </param>
        /// <param name="oracleContext"> Context for the oracle. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public TinyWebApiRepository(ILogger<TinyWebApiRepository> logger, IDBContextSql sqlContext, IDBContextOracle oracleContext) : base(sqlContext, oracleContext) => _logger = logger;
        /// <summary>
        ///     Gets this message.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="executionType">         Type of the execution. </param>
        /// <param name="outPutType">            Type of the out put. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public dynamic Get(string key, List<RequestSpecification> requestSpecifications, ExecutionType executionType, OutPutType outPutType) => this.Execute(key, requestSpecifications, executionType, outPutType);
        /// <summary>
        ///     Post this message.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="executionType">         Type of the execution. </param>
        /// <param name="outPutType">            Type of the out put. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public dynamic Post(string key, List<RequestSpecification> requestSpecifications, ExecutionType executionType, OutPutType outPutType) => this.Execute(key, requestSpecifications, executionType, outPutType);
        /// <summary>
        ///     Executes.
        /// </summary>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <exception cref="CustomException">   Thrown when a Custom error condition occurs. </exception>
        /// <param name="key">                   The key. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="executionType">         Type of the execution. </param>
        /// <param name="outPutType">            Type of the out put. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public dynamic Execute(string key, List<RequestSpecification> requestSpecifications, ExecutionType executionType, OutPutType outPutType)
        {
            Global.LogInformation("Inside Execute query, validate key.");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"Key cannot be null or empty.");
            try
            {
                Global.LogInformation("Gey query specification by query name/key.");
                var querySpecification = Global.GetQuerySpecificationByQueryName(key);
                if (querySpecification is null) throw new ArgumentException($"Query not mapped for the key : {key}.");
                var isDoNotFireFurtherQuery = false;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                object output = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Global.LogInformation("Processing input data before firing database queries.");
#pragma warning disable CS8601 // Possible null reference assignment.
                requestSpecifications = requestSpecifications.ProcessInputData(key, querySpecification, ref isDoNotFireFurtherQuery, ref output);
#pragma warning restore CS8601 // Possible null reference assignment.
                if (isDoNotFireFurtherQuery)
                {
                    Global.LogInformation("Not firing the queries and escaping it.");
                    return output is null ? 0 : output;
                }
                else
                {
                    Global.LogInformation("Get parameters.");
                    if (querySpecification.DatabaseSpecification is null) throw new ArgumentException($"Database not mapped for the query name : {key}.");
                    var list = GetParameters(requestSpecifications);
                    Global.LogInformation("Process parameters.");
                    list = ProcessParameters(querySpecification, list);
                    Global.LogInformation("Processing query.");
                    querySpecification.Query = ProcessQuery(querySpecification, list, executionType);
                    Global.LogInformation("Executing query and returning result.");
                    return ExecuteQuery(key, querySpecification, executionType, list, requestSpecifications, outPutType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while trying to serve request...");
                throw new CustomException((int)HttpStatusCode.InternalServerError, "Internal Server Error!!!", $"Something went wrong while trying to serve request...{Environment.NewLine}Error:{ex.Message + ""}");
            }
        }
    }
}
