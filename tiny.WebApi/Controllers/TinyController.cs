// <copyright file="tiny.WebApiController.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the Tiny Web API controller class.
// </summary>
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using tiny.WebApi.Enums;
using tiny.WebApi.IDataContracts;
using tiny.WebApi.IDBContext;
using tiny.WebApi.IRepository;
using static tiny.WebApi.Helpers.MailClient;
namespace tiny.WebApi.Controllers
{
    /// <summary>
    ///     A controller for handling tiny controller calls.
    /// </summary>
    /// <seealso cref="T:Base"/>
    [DebuggerStepThrough]
    public class TinyWebApiController : Base
    {
        /// <summary>
        ///     (Immutable) an Itiny.WebApiRepository to process.
        /// </summary>
        private readonly ITinyWebApiRepository r;
        /// <summary>
        ///     (Immutable) the.
        /// </summary>
        private readonly IBase _;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.Controllers.tiny.WebApiController class.
        /// </summary>
        /// <param name="repository">    The repository. </param>
        /// <param name="logger">        The logger. </param>
        /// <param name="sqlContext">    Context for the SQL. </param>
        /// <param name="oracleContext"> Context for the oracle. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public TinyWebApiController(ITinyWebApiRepository repository, ILogger<TinyWebApiController> logger, IDBContextSql sqlContext, IDBContextOracle oracleContext) : base(logger, sqlContext, oracleContext)
        {
            r = repository;
            _ = this;
        }
        /// <summary>
        ///     (An Action that handles HTTP POST requests) (Restricted to ) post this message.
        ///     Route : TinyWebApi/Post/{key}/{executionType}/{outPutType?}/{hasFileContent?}/{fileContentType?}/{fileContentFieldName?}/{sheetName?}
        /// </summary>
        /// <param name="key">                  The key. </param>
        /// <param name="request">              The request. </param>
        /// <param name="executionType">        Type of the execution. </param>
        /// <param name="token">                A token that allows processing to be cancelled. </param>
        /// <param name="outPutType">           (Optional) Type of the out put. </param>
        /// <param name="hasFileContent">       (Optional) True if has file content, false if not. </param>
        /// <param name="fileContentType">      (Optional) Type of the file content. </param>
        /// <param name="fileContentFieldName"> (Optional) Name of the file content field. </param>
        /// <param name="sheetName">            (Optional) Name of the sheet. </param>
        /// <returns>
        ///     An IActionResult.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        [AcceptVerbs("POST","PUT", "DELETE")]
        [EnableCors("CorsPolicy")]
        [Route("[controller]/Post/{key}/{executionType}/{outPutType?}/{hasFileContent?}/{fileContentType?}/{fileContentFieldName?}/{sheetName?}")]
        [Route("[controller]/Put/{key}/{executionType}/{outPutType?}/{hasFileContent?}/{fileContentType?}/{fileContentFieldName?}/{sheetName?}")]
        [Route("[controller]/Delete/{key}/{executionType}/{outPutType?}/{hasFileContent?}/{fileContentType?}/{fileContentFieldName?}/{sheetName?}")]
        public virtual async Task<IActionResult> Post(string key, [FromBody] dynamic request, ExecutionType executionType, CancellationToken token, OutPutType outPutType = OutPutType.JSON, bool hasFileContent = false, FileContentType fileContentType = FileContentType.Excel, string fileContentFieldName = "", string sheetName = "") => await _.ExecuteAsync(() => _.MapOutPutType(outPutType, executionType).OutPutType == OutPutType.JSON ? Ok(r.Post(key, _.GetRequestSpecification(request, executionType, outPutType, hasFileContent, fileContentType, fileContentFieldName, sheetName), executionType, outPutType)) : File(r.Post(key, _.GetRequestSpecification(request, executionType, outPutType, hasFileContent, fileContentType, fileContentFieldName, sheetName), executionType, outPutType), _.OutPutType == OutPutType.Excel ? ApplicationExcel : _.OutPutType == OutPutType.PDF ? ApplicationPDF : TextCSV, Guid.NewGuid().ToString() + (_.OutPutType == OutPutType.Excel ? ExcelExtension : _.OutPutType == OutPutType.PDF ? PDFExtension : CSVExtension)), token);
        /// <summary>
        ///     (An Action that handles HTTP GET requests) (Restricted to ) gets.
        ///     Route : TinyWebApi/Get/{key}/{executionType}/{outPutType?}/{hasFileContent?}/{fileContentType?}/{fileContentFieldName?}/{sheetName?}
        /// </summary>
        /// <param name="key">           The key. </param>
        /// <param name="executionType"> Type of the execution. </param>
        /// <param name="token">         A token that allows processing to be cancelled. </param>
        /// <param name="outPutType">    (Optional) Type of the out put. </param>
        /// <returns>
        ///     An IActionResult.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        [HttpGet]
        [EnableCors("CorsPolicy")]
        [Route("[controller]/Get/{key}/{executionType}/{outPutType?}")]
        public virtual async Task<IActionResult> Get(string key, ExecutionType executionType, CancellationToken token, OutPutType outPutType = OutPutType.JSON) => await _.ExecuteAsync(() => _.MapOutPutType(outPutType, executionType).OutPutType == OutPutType.JSON ? Ok(r.Get(key, _.GetRequestSpecificationFromQueryParameters(), executionType, outPutType)) : File(r.Get(key, _.GetRequestSpecificationFromQueryParameters(), executionType, outPutType), _.OutPutType == OutPutType.Excel ? ApplicationExcel : _.OutPutType == OutPutType.PDF ? ApplicationPDF : TextCSV, Guid.NewGuid().ToString() + (_.OutPutType == OutPutType.Excel ? ExcelExtension : _.OutPutType == OutPutType.PDF ? PDFExtension : CSVExtension)), token);
    }
}
