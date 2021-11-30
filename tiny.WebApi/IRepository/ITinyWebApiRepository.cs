// <copyright file="Itiny.WebApiRepository.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the itiny web API repository class.
// </summary>
using System.Collections.Generic;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
namespace tiny.WebApi.IRepository
{
    /// <summary>
    ///     Interface for ITiny web API repository.
    /// </summary>
    public interface ITinyWebApiRepository
    {
        /// <summary>
        ///     Post this message.
        /// </summary>
        /// <param name="key">           The key. </param>
        /// <param name="request">       The request. </param>
        /// <param name="executionType"> Type of the execution. </param>
        /// <param name="outPutType">    Type of the out put. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        dynamic Post(string key, List<RequestSpecification> request, ExecutionType executionType, OutPutType outPutType);
        /// <summary>
        ///     Gets.
        /// </summary>
        /// <param name="key">           The key. </param>
        /// <param name="request">       The request. </param>
        /// <param name="executionType"> Type of the execution. </param>
        /// <param name="outPutType">    Type of the out put. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        dynamic Get(string key, List<RequestSpecification> request, ExecutionType executionType, OutPutType outPutType);
    }
}