// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
namespace tiny.WebApi.Exceptions
{
    /// <summary>
    ///     An operation cancelled exception filter.
    /// </summary>
    /// <seealso cref="T:ExceptionFilterAttribute"/>
    [DebuggerStepThrough]
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        ///     (Immutable) the logger.
        /// </summary>
        private readonly ILogger<OperationCancelledExceptionFilter> _logger;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.OperationCancelledExceptionFilter class.
        /// </summary>
        /// <param name="loggerFactory"> The logger factory. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public OperationCancelledExceptionFilter(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger<OperationCancelledExceptionFilter>();
        /// <summary>
        ///     Executes the 'exception' action.
        /// </summary>
        /// <seealso cref="M:Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute.OnException(ExceptionContext)"/>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled.");
                context.ExceptionHandled = true;
                context.Result = new StatusCodeResult(400);
            }
        }
    }
}
