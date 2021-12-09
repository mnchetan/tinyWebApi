// <copyright file="RequestSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using tiny.WebApi.Enums;
namespace tiny.WebApi.DataObjects
{
    /// <summary>
    ///     A request specification.
    /// </summary>
    [DebuggerStepThrough]
    public class RequestSpecification
    {
        /// <summary>
        ///     Gets a value indicating whether this object is file content.
        /// </summary>
        /// <value>
        ///     True if this object is file content, false if not.
        /// </value>
        [DebuggerHidden]
        public bool IsFileContent { get; set; }
        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <value>
        ///     The name of the property.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string PropertyName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets the call type.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string CallType { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets a value indicating whether this object is array of multiple fields.
        /// </summary>
        /// <value>
        ///     True if this object is array of multiple fields, false if not.
        /// </value>
        [DebuggerHidden]
        public bool IsArrayOfMultipleFields { get; set; }
        /// <summary>
        /// Gets the file content type.
        /// </summary>
        [DebuggerHidden]
        public FileContentType FileContentType { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is array of single field.
        /// </summary>
        ///
        /// <value>
        ///     True if this object is array of single field, false if not.
        /// </value>
        [DebuggerHidden]
        public bool IsArrayOfSingleField { get; set; }
        /// <summary>
        ///     Gets the type of the property.
        /// </summary>
        /// <value>
        ///     The type of the property.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Type PropertyType { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <value>
        ///     The property value.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public dynamic PropertyValue { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the type of the database parameter.
        /// </summary>
        /// <value>
        ///     The type of the database parameter.
        /// </value>
        [DebuggerHidden]
        public DatabaseParameterType DatabaseParameterType => IsFileContent ? FileContentType == FileContentType.BLOB ? DatabaseParameterType.Binary : DatabaseParameterType.Structured : PropertyType is null ? CallType == "P" ? DatabaseParameterType.UnKnown : DatabaseParameterType.String : PropertyType.Name switch { nameof(Object) => DatabaseParameterType.Object, nameof(String) => DatabaseParameterType.String, nameof(Boolean) => DatabaseParameterType.Boolean, nameof(DateTime) or nameof(TimeSpan) => DatabaseParameterType.DateTime, nameof(Int64) => DatabaseParameterType.Int64, nameof(Decimal) => DatabaseParameterType.Decimal, nameof(DataTable) => DatabaseParameterType.Structured, _ => DatabaseParameterType.String };
        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        internal dynamic Value => IsFileContent ? PropertyValue : IsArrayOfSingleField && !IsArrayOfMultipleFields ? PropertyType.Name switch { nameof(Object) => PropertyValue, nameof(String) or nameof(DateTime) or nameof(TimeSpan) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<string>).Select(o => "'" + o.Replace("'", "''") + "'")) : PropertyValue, nameof(Int64) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<Int32>).Select(o => o)) : PropertyValue, nameof(Decimal) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<Decimal>).Select(o => o)) : PropertyValue, _ => PropertyValue, } : IsArrayOfSingleField && IsArrayOfMultipleFields ? (DataTable)JsonConvert.DeserializeObject(Convert.ToString(PropertyValue), typeof(DataTable)) : CallType == "P" && (PropertyValue is null || string.IsNullOrEmpty(Convert.ToString(PropertyValue))) ? null : CallType == "G" ? Convert.ToString(PropertyValue).Contains(",") ? string.Join(',', (PropertyValue.Split(',') as string[]).Select(o => "'" + o.Replace("'", "''") + "'")) : string.IsNullOrEmpty(Convert.ToString(PropertyValue)) ? "" : Convert.ToString(PropertyValue) : string.IsNullOrEmpty(Convert.ToString(PropertyValue)) ? "" : Convert.ToString(PropertyValue);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
    }
}