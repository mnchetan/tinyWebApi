// <copyright file="DatabaseParameters.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the database parameters class.
// </summary>
using tinyWebApi.Common.Enums;
using System.Diagnostics;
namespace tinyWebApi.Common.DataObjects
{
    /// <summary>
    ///     A database parameters.
    /// </summary>
    [DebuggerStepThrough]
    public class DatabaseParameters
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [DebuggerHidden]
        public string Name { get; set; }
        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        [DebuggerHidden]
        public object? Value { get; set; }
        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        [DebuggerHidden]
        public DatabaseParameterType? Type { get; set; }
        /// <summary>
        ///     Gets or sets a value indicating whether this object is out parameter.
        /// </summary>
        /// <value>
        ///     True if this object is out parameter, false if not.
        /// </value>
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        [DebuggerHidden]
        public bool IsOutParameter { get; set; }
        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        [DebuggerHidden]
        public int Size { get; set; }
        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag value is used to store the UDT type.
        /// </value>
        [DebuggerHidden]
        public string Tag { get; set; }
    }
}
