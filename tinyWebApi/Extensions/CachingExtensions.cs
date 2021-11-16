// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Helpers;
namespace tinyWebApi.Common.Extensions
{
    /// <summary>
    ///     The caching extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class CachingExtensions
    {
        /// <summary>
        /// Lock object.
        /// </summary>
        private static readonly object _lockObject = new();
        /// <summary>
        /// Gets or Sets the Tiny Cache.
        /// </summary>
        [DebuggerHidden]
        private static Dictionary<string, CachingHelper> TinyCache { get; set; } = new();
        /// <summary>
        /// Confirms if caching is enabled or not.
        /// </summary>
        /// <param name="querySpecification"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static bool CachingEnabled(this QuerySpecification querySpecification) => querySpecification.IsCachingRequired && querySpecification.CacheDurationInSeconds > 0;
        /// <summary>
        /// Confirms if allowed to serve from cache.
        /// </summary>
        /// <param name="querySpecification"></param>
        /// <param name="executionType"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static bool AllowServeFromCache(this QuerySpecification querySpecification, ExecutionType executionType) => TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) != null && (TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()).LastFetchedFromDatabaseOnDateTimeInUTC - DateTime.UtcNow) <= new TimeSpan(0, 0, querySpecification.CacheDurationInSeconds);
        /// <summary>
        /// Add to Cache.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="querySpecification"></param>
        /// <param name="executionType"></param>
        /// <returns>System.Data.DataSet</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static DataSet AddToCache(this DataSet data, QuerySpecification querySpecification, ExecutionType executionType)
        {
            Global.LogInformation("Inside AddToCache, if data exists then delete data and then add new data to cache in thread safe manner and then return the same data back.");
            if (querySpecification.CachingEnabled())
            {
                lock (_lockObject)
                {
                    if (TinyCache.ContainsKey($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()))
                        TinyCache.Remove($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower());
                    TinyCache.Add($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower(), new CachingHelper() { CachedData = data, LastFetchedFromDatabaseOnDateTimeInUTC = DateTime.UtcNow, QuerySpecification = querySpecification });
                    return data;
                }
            }
            else
                return data;
        }
        /// <summary>
        /// Add to Cache.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="querySpecification"></param>
        /// <param name="executionType"></param>
        /// <returns>System.Data.DataTable</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static DataTable AddToCache(this DataTable data, QuerySpecification querySpecification, ExecutionType executionType)
        {
            Global.LogInformation("Inside AddToCache, if data exists then delete data and then add new data to cache in thread safe manner and then return the same data back.");
            if (querySpecification.CachingEnabled())
            {
                lock (_lockObject)
                {
                    if (TinyCache.ContainsKey($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()))
                        TinyCache.Remove($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower());
                    TinyCache.Add($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower(), new CachingHelper() { CachedData = data, LastFetchedFromDatabaseOnDateTimeInUTC = DateTime.UtcNow, QuerySpecification = querySpecification });
                    return data;
                }
            }
            else
                return data;
        }
        /// <summary>
        /// Serve from Cache.
        /// </summary>
        /// <param name="querySpecification"></param>
        /// <param name="executionType"></param>
        /// <returns>System.Data.DataTable</returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static T ServeFromCache<T>(this QuerySpecification querySpecification, ExecutionType executionType)
        {
            Global.LogInformation($"Inside ServeFromCache, if data exists return data else return instance of {typeof(T)}.");
            var value = TinyCache.ContainsKey($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) && TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) != null ? TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()).CachedData : Activator.CreateInstance<T>();
            return value switch
            {
                DataSet => JsonConvert.DeserializeObject<DataSet>((value as DataSet).ToJSON()),
                DataTable => JsonConvert.DeserializeObject<DataTable>((value as DataTable).ToJSON()),
                _ => value,
            };
        }
    }
}
