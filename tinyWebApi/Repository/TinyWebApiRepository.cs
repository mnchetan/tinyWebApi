/// <copyright file="tinyWebApiRepository.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the tiny web API repository class.
/// </summary>
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.Exceptions;
using tinyWebApi.Common.Extensions;
using tinyWebApi.Common.IDBContext;
using tinyWebApi.Common.IRepository;
namespace tinyWebApi.Common
{
    /// <summary>
    ///     A tiny web API repository.
    /// </summary>
    /// <seealso cref="T:BaseRepository"/>
    /// <seealso cref="T:ItinyWebApiRepository"/>
    [DebuggerStepThrough]
    public class TinyWebApiRepository : BaseRepository, ITinyWebApiRepository
    {
        /// <summary>
        ///     (Immutable) the logger.
        /// </summary>
        private readonly ILogger<TinyWebApiRepository> _logger;
        /// <summary>
        ///     Initializes a new instance of the tinyWebApi.Common.tinyWebApiRepository class.
        /// </summary>
        /// <param name="logger">        (Immutable) the logger. </param>
        /// <param name="sqlContext">    Context for the SQL. </param>
        /// <param name="oracleContext"> Context for the oracle. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
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
        [DebuggerHidden]
        [DebuggerStepThrough]
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
        [DebuggerHidden]
        [DebuggerStepThrough]
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
        [DebuggerHidden]
        [DebuggerStepThrough]
        public dynamic Execute(string key, List<RequestSpecification> requestSpecifications, ExecutionType executionType, OutPutType outPutType)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"Key cannot be null or empty.");
            try
            {
                var querySpecification = Global.GetQuerySpecificationByQueryName(key);
                if (querySpecification is null) throw new ArgumentException($"Query not mapped for the key : {key}.");
                var isDoNotFireFurtherQuery = false;
                object output = null;
                requestSpecifications = requestSpecifications.ProcessInputData(key, querySpecification, ref isDoNotFireFurtherQuery, ref output);
                if (isDoNotFireFurtherQuery) return output is null ? 0 : output;
                else
                {
                    if (querySpecification.DatabaseSpecification is null) throw new ArgumentException($"Database not mapped for the query name : {key}.");
                    var list = GetParameters(requestSpecifications);
                    list = ProcessParameters(querySpecification, list);
                    querySpecification.Query = ProcessQuery(querySpecification, list, executionType);
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
