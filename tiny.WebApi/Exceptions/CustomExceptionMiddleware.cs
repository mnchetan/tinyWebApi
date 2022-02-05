// <copyright file="CustomExceptionMiddleware.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the custom exception middleware class.
// </summary>
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Exceptions
{
    /// <summary>
    ///     A custom exception middleware.
    /// </summary>
    [DebuggerStepThrough]
    public class CustomExceptionMiddleware
    {
        /// <summary>
        ///     (Immutable) the next.
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.CustomExceptionMiddleware class.
        /// </summary>
        /// <param name="next"> (Immutable) the next. </param>
        [DebuggerStepThrough]
        public CustomExceptionMiddleware(RequestDelegate next) => _next = next;
        /// <summary>
        ///     Executes the given operation on a different thread, and waits for the result.
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <returns>
        ///     A Task.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        /// <summary>
        ///     Handles the exception asynchronous.
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <param name="ex">      The exception. </param>
        /// <returns>
        ///     A Task.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            Global.LogDebug("Inside HandleExceptionAsync, prepare custom exception and return.");
            int statusCode;
            string description, message;
            if (ex is CustomException exception)
            {
                message = exception.Message;
                description = exception.Description;
                statusCode = exception.Code;
            }
            else
            {
                message = ex.Message;
                description = ex.StackTrace ?? "Unexpected error";
                statusCode = (int)HttpStatusCode.InternalServerError;
            }
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                statusCode,
                message,
                description,
                Exception = ex
            }));
        }
    }
}
