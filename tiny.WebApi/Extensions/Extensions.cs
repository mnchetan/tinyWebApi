// <copyright file="Extensions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using tiny.WebApi.DataObjects;

namespace tiny.WebApi.Extensions
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
        [DebuggerStepThrough]
        [DebuggerHidden]
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
        /// <summary>
        /// Enumerate Data Table as List of dynamic.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static List<dynamic> AsDynamicEnumerable(this DataTable dt)
        {
            List<dynamic> expandoList = new();
            foreach (var (row, expandoDict) in from DataRow row in dt.Rows//create a new ExpandoObject() at each row
                                               let expandoDict = new ExpandoObject() as IDictionary<string, object>
                                               select (row, expandoDict))
            {
                foreach (DataColumn col in dt.Columns)
                {
                    //put every column of this row into the new dictionary
                    expandoDict.Add(Regex.Replace(col.ColumnName, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled), row[col.ColumnName] is not null && row[col.ColumnName] != DBNull.Value ? Convert.ToString(row[col.ColumnName]) : string.Empty);
                }
                //add this "row" to the list
                expandoList.Add(expandoDict);
            }

            return expandoList;
        }
    }
}
