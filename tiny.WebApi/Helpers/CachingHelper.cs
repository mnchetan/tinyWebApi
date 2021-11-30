// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using System;
using System.Diagnostics;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    /// Caching helper.
    /// </summary>
    [DebuggerStepThrough]
    internal class CachingHelper
    {
        /// <summary>
        /// Last fetched from database on datetime in UTC to infer if cache window has been expired or not.
        /// </summary>
        [DebuggerHidden]
        internal DateTime LastFetchedFromDatabaseOnDateTimeInUTC { get; set; }
        /// <summary>
        /// Gets or Sets the cached data.
        /// </summary>
        [DebuggerHidden]
        internal dynamic CachedData { get; set; }
        /// <summary>
        /// Gets or Sets the Query Specification.
        /// </summary>
        [DebuggerHidden]
        internal QuerySpecification QuerySpecification { get; set; }
    }
}
