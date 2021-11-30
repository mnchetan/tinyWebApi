// <copyright file="RequestSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using tiny.WebApi.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
        public string PropertyName { get; set; }
        /// <summary>
        /// Gets the call type.
        /// </summary>
        [DebuggerHidden]
        public string CallType { get; set; }
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
        public Type PropertyType { get; set; }
        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <value>
        ///     The property value.
        /// </value>
        [DebuggerHidden]
        public dynamic PropertyValue { get; set; }
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
        public dynamic Value => IsFileContent ? PropertyValue : IsArrayOfSingleField && !IsArrayOfMultipleFields ? PropertyType.Name switch { nameof(Object) => PropertyValue, nameof(String) or nameof(DateTime) or nameof(TimeSpan) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<string>).Select(o => "'" + o.Replace("'", "''") + "'")) : PropertyValue, nameof(Int64) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<Int32>).Select(o => o)) : PropertyValue, nameof(Decimal) => PropertyValue is List<String> ? string.Join(',', (PropertyValue as List<Decimal>).Select(o => o)) : PropertyValue, _ => PropertyValue, } : IsArrayOfSingleField && IsArrayOfMultipleFields ? (DataTable)JsonConvert.DeserializeObject(Convert.ToString(PropertyValue), typeof(DataTable)) : CallType == "P" && (PropertyValue is null || string.IsNullOrEmpty(Convert.ToString(PropertyValue))) ? null : CallType == "G" ? Convert.ToString(PropertyValue).Contains(",") ? string.Join(',', (PropertyValue.Split(',') as string[]).Select(o => "'" + o.Replace("'", "''") + "'")) : string.IsNullOrEmpty(Convert.ToString(PropertyValue)) ? "" : Convert.ToString(PropertyValue) : string.IsNullOrEmpty(Convert.ToString(PropertyValue)) ? "" : Convert.ToString(PropertyValue);
    }
}