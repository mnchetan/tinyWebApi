/// <copyright file="Extensions.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using Newtonsoft.Json;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using tinyWebApi.Common.DataObjects;

namespace tinyWebApi.Common.Extensions
{
    /// <summary>
    /// The extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class Extensions
    {
        /// <summary>
        ///     A DataTable extension method that data table to CSV.
        /// </summary>
        /// <param name="dt"> The dt to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static dynamic DataTableToCSV(this DataTable dt)
        {
            Global.LogInformation("Inside DataTableToCSV, Converting DataTable to csv and taking in account the double quotes in data.");
            StringBuilder sb = new();
            var columnNames = dt.Columns.Cast<DataColumn>().Select(o => o.ColumnName.Replace("\"", "\"\""));
            _ = sb.AppendLine(string.Join(",", columnNames));
            foreach (var fields in from DataRow row in dt.Rows let fields = row.ItemArray.Select(fields => string.Concat("\"", fields.ToString().Replace("\"", "\"\""), "\"")).ToList() select fields) _ = sb.AppendLine(string.Join(",", fields));
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        /// <summary>
        ///     An object extension method that converts a content to a JSON.
        /// </summary>
        /// <param name="content"> The content to act on. </param>
        /// <returns>
        ///     Content as a dynamic.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static dynamic ToJSON(this object content) => JsonConvert.SerializeObject(content, Formatting.Indented);
        /// <summary>
        ///     A DataRow extension method that gets value or default.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="row">       The row to act on. </param>
        /// <param name="fieldName"> Name of the field. </param>
        /// <returns>
        ///     The value or default.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static T GetValueOrDefault<T>(this DataRow row, string fieldName)
        {
            Global.LogInformation($"Inside GetValueOrDefault, Getting the value for specified DataColumn of the DataRow in scope and if not found the return instance of {typeof(T)}.");
            if (row.Table.Columns.Contains(fieldName) && !row[fieldName].Equals(DBNull.Value) && !row[fieldName].Equals(null))
            {
                switch (row[fieldName])
                {
                    case T:
                        return (T)row[fieldName];
                    default:
                        try
                        {
                            return (T)Convert.ChangeType(row[fieldName], typeof(T));
                        }
                        catch
                        {
                            return default;
                        }
                }
            }
            else return default;
        }
    }
}
