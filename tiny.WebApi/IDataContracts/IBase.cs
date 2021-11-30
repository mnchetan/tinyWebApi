// <copyright file="IBase.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
namespace tiny.WebApi.IDataContracts
{
    /// <summary>
    ///     Interface for base.
    /// </summary>
    public interface IBase
    {
        /// <summary>
        ///     Executes the 'asynchronous' operation.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="f">                 A Func&lt;T&gt; to process. </param>
        /// <param name="cancellationToken"> A token that allows processing to be cancelled. </param>
        /// <param name="callerName">        (Optional) Name of the caller. </param>
        /// <param name="callerFilePath">    (Optional) Full pathname of the caller file. </param>
        /// <param name="callerLineNumer">   (Optional) The caller line numer. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        Task<T> ExecuteAsync<T>(Func<T> f, CancellationToken cancellationToken, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumer = 0);
        /// <summary>
        ///     Executes the 'asynchronous' operation.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="a">               An Action to process. </param>
        /// <param name="cancellation">    A token that allows processing to be cancelled. </param>
        /// <param name="callerName">      (Optional) Name of the caller. </param>
        /// <param name="callerFilePath">  (Optional) Full pathname of the caller file. </param>
        /// <param name="callerLineNumer"> (Optional) The caller line numer. </param>
        void ExecuteAsync<T>(Action a, CancellationToken cancellation, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumer = 0);
        /// <summary>
        ///     Gets request specification.
        /// </summary>
        /// <param name="request">              The request. </param>
        /// <param name="executionType">        Type of the execution. </param>
        /// <param name="outPutType">           (Optional) Type of the out put. </param>
        /// <param name="hasFileContent">       (Optional) True if has file content, false if not. </param>
        /// <param name="fileContentType">      (Optional) Type of the file content. </param>
        /// <param name="fileContentFieldName"> (Optional) Name of the file content field. </param>
        /// <param name="sheetName">            (Optional) Name of the sheet. </param>
        /// <returns>
        ///     The request specification.
        /// </returns>
        List<RequestSpecification> GetRequestSpecification(dynamic request, ExecutionType executionType, OutPutType outPutType = OutPutType.JSON, bool hasFileContent = false, FileContentType  fileContentType = FileContentType.Excel, string fileContentFieldName = "", string sheetName = "");
        /// <summary>
        ///     Gets request specification from query parameters.
        /// </summary>
        /// <returns>
        ///     The request specification from query parameters.
        /// </returns>
        List<RequestSpecification> GetRequestSpecificationFromQueryParameters();
        /// <summary>
        ///     Map out put type.
        /// </summary>
        /// <param name="outPutType">    Type of the out put. </param>
        /// <param name="executionType"> Type of the execution. </param>
        /// <returns>
        ///     An IBase.
        /// </returns>
        IBase MapOutPutType(OutPutType outPutType, ExecutionType executionType);
        /// <summary>
        ///     Gets or sets the type of the out put.
        /// </summary>
        /// <value>
        ///     The type of the out put.
        /// </value>
        OutPutType OutPutType { get; set; }
    }
}
