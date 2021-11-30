// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Diagnostics;
namespace tiny.WebApi.Exceptions
{
    /// <summary>
    ///     (Serializable) exception for signalling custom errors.
    /// </summary>
    /// <seealso cref="T:Exception"/>
    [DebuggerStepThrough]
    [Serializable]
    public class CustomException : Exception
    {
        /// <summary>
        ///     Gets or sets the code.
        /// </summary>
        /// <value>
        ///     The code.
        /// </value>
        [DebuggerHidden]
        public int Code { get; set; }
        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        [DebuggerHidden]
        public string Description { get; set; }
        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        [DebuggerHidden]
        public new string Message { get; set; }
        /// <summary>
        ///     Initializes a new instance of the tiny.WebApi.Exceptions.CustomException class.
        /// </summary>
        /// <param name="code">        The code. </param>
        /// <param name="message">     The message. </param>
        /// <param name="description"> The description. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public CustomException(int code, string message, string description)
        {
            Code = code;
            Description = description;
            Message = message;
        }
    }
}
