// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
using tiny.WebApi.Helpers;
namespace tiny.WebApi.Extensions
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        internal static bool AllowServeFromCache(this QuerySpecification querySpecification, ExecutionType executionType) => TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) != null && (TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()).LastFetchedFromDatabaseOnDateTimeInUTC - DateTime.UtcNow) <= new TimeSpan(0, 0, querySpecification.CacheDurationInSeconds);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var value = TinyCache.ContainsKey($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) && TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()) != null ? TinyCache.GetValueOrDefault($"{querySpecification.Query}_ {Enum.GetName(executionType)}".ToLower()).CachedData : Activator.CreateInstance<T>();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
            return value switch
            {
#pragma warning disable CS8604 // Possible null reference argument.
                DataSet => JsonConvert.DeserializeObject<DataSet>((value as DataSet).ToJSON()),
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
                DataTable => JsonConvert.DeserializeObject<DataTable>((value as DataTable).ToJSON()),
#pragma warning restore CS8604 // Possible null reference argument.
                _ => value,
            };
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
