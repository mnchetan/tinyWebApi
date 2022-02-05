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
            Global.LogDebug("Inside DataTableToCSV, Converting DataTable to csv and taking in account the double quotes in data.");
            StringBuilder sb = new();
            var columnNames = dt.Columns.Cast<DataColumn>().Select(o => o.ColumnName.Replace("\"", "\"\""));
            _ = sb.AppendLine(string.Join(",", columnNames));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var fields in from DataRow row in dt.Rows let fields = row.ItemArray.Select(fields => string.Concat("\"", fields.ToString().Replace("\"", "\"\""), "\"")).ToList() select fields) _ = sb.AppendLine(string.Join(",", fields));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
            Global.LogDebug($"Inside GetValueOrDefault, Getting the value for specified DataColumn of the DataRow in scope and if not found the return instance of {typeof(T)}.");
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
#pragma warning disable CS8603 // Possible null reference return.
                            return default;
#pragma warning restore CS8603 // Possible null reference return.
                        }
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            else return default;
#pragma warning restore CS8603 // Possible null reference return.
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
        /// <summary>
        /// Get value or default from the dictionary with culture neutral and case neutral key if key is of type string.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public static TValue GetValueOrDefaultIgnoreCase<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) => key is not null ? key is string ? dictionary.FirstOrDefault(o => (o.Key as string).ToLowerInvariant() == (key as string).ToLowerInvariant()).Value : dictionary.GetValueOrDefault(key) : default;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        /// Checks if dictioanry contains the provided key where key is looked up in a case neutral way if key is of type string.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public static bool ContainsKeyIgnoreCase<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) => key is not null && (key is string ? dictionary.Keys.OfType<string>().Any(k => string.Equals(k, key as string, StringComparison.OrdinalIgnoreCase)) : dictionary.ContainsKey(key));
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static bool RemoveIgnoreCase<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            switch (key)
            {
                case not null:
                    if (key is string)
                    {
                        bool isFound = false;
                        KeyValuePair<TKey, TValue> actualKey = new();
                        foreach (var item in from item in dictionary where (item.Key as string).ToLowerInvariant() == (key as string).ToLowerInvariant() select item)
                        {
                            isFound = true;
                            actualKey = item;
                            break;
                        }

                        return isFound && dictionary.Remove(actualKey.Key);
                    }
                    else return dictionary.Remove(key);
                default: return false;
            }
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}