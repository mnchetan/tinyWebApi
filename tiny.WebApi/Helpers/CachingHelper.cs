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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal dynamic CachedData { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets or Sets the Query Specification.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal QuerySpecification QuerySpecification { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
