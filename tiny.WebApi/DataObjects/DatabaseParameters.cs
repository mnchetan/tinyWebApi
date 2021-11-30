// <copyright file="DatabaseParameters.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the database parameters class.
// </summary>
using System.Diagnostics;
using tiny.WebApi.Enums;
namespace tiny.WebApi.DataObjects
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Tag { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
