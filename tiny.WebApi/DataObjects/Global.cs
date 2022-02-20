// <copyright file="Global.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using tiny.WebApi.Configurations;
using static tiny.Logger.Extensions;
namespace tiny.WebApi.DataObjects
{
    /// <summary>
    ///     A global.
    /// </summary>
    [DebuggerStepThrough]
    public static class Global
    {
        /// <summary>
        ///     Gets the full path name of the configuration directory.
        ///     Fetched from tiny.WebApi.Configurations.Itiny.WebApiConfigurations.ConfigurationDirectoryPath and is overriden by appsettings.environment.json and if bot not specified then executing assembly path is taken up.
        /// </summary>
        /// <value>
        ///     The full path name of the configuration directory.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public static string ConfigurationDirectoryPath => !string.IsNullOrEmpty(Configuration?.GetSection("ConfigurationDirectoryPath").Value) && Directory.Exists(Configuration?.GetSection("ConfigurationDirectoryPath").Value) ? Configuration?.GetSection("AllowedCorsHosts").Value : !string.IsNullOrEmpty(TinyWebApiConfigurations.ConfigurationDirectoryPath) && Directory.Exists(TinyWebApiConfigurations.ConfigurationDirectoryPath) ? TinyWebApiConfigurations.ConfigurationDirectoryPath : new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        ///     Logs an information.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogInformation(string message, object objToLog = null) => Log(LogLevel.Information, message, objToLog);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objToLog">The obj to log.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="ex">The ex.</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogCritical(string message, object objToLog = default, EventId eventId = default, Exception ex = default) { Log(LogLevel.Critical, message, ex); Log(LogLevel.Critical, message, objToLog); Log(LogLevel.Critical, message, eventId); }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Logs a trace.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogTrace(string message, object objToLog = null) => Log(LogLevel.Trace, message, objToLog);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Logs a warning.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogWarning(string message, object objToLog = null) => Log(LogLevel.Warning, message, objToLog);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Logs a critical.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogCritical(string message, object objToLog = null) => Log(LogLevel.Critical, message, objToLog);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Logs a debug.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogDebug(string message, object objToLog = null) => Log(LogLevel.Debug, message, objToLog);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Logs an error.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="ex">       The exception. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void LogError(string message, Exception ex, object objToLog = null) { Log(LogLevel.Error, message, ex); Log(LogLevel.Error, message, objToLog); }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Gets the environment.
        ///     Default environment is "Production".
        /// </summary>
        /// <value>
        ///     The environment.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public static string Environment => string.IsNullOrEmpty(WebHostingEnvironment.EnvironmentName) ? "Production" : WebHostingEnvironment.EnvironmentName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        /// <summary>
        ///     Gets the run as user specifications.
        /// </summary>
        /// <value>
        ///     The run as user specifications.
        /// </value>
        [DebuggerHidden]
        public static Dictionary<string, RunAsUserSpecification> RunAsUserSpecifications
        {
            get
            {
                LogInformation("Returning the RunAsUserSpecification from JSON file or Pre-Configured code or merge of both and if not available then default.");
                Dictionary<string, RunAsUserSpecification> result = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (File.Exists(TinyWebApiConfigurations.RunAsUserJSONFilePath))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    result = JsonConvert.DeserializeObject<Dictionary<string, RunAsUserSpecification>>(Helpers.FileReadWriteHelper.ReadAllText(TinyWebApiConfigurations.RunAsUserJSONFilePath));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (TinyWebApiConfigurations.RunAsUserSpecifications is not null && TinyWebApiConfigurations.RunAsUserSpecifications.Count > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    foreach (var item in TinyWebApiConfigurations.RunAsUserSpecifications.Where(item => !result.ContainsKey(item.Key)))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        result.Add(item.Key, item.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        /// <summary>
        ///     Gets the database specifications.
        /// </summary>
        /// <value>
        ///     The database specifications.
        /// </value>
        [DebuggerHidden]
        public static Dictionary<string, DatabaseSpecification> DatabaseSpecifications
        {
            get
            {
                LogInformation("Returning the DatabaseSpecification from JSON file or Pre-Configured code or merge of both and if not available then default.");
                Dictionary<string, DatabaseSpecification> result = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (File.Exists(TinyWebApiConfigurations.ConnectionStringJSONFilePath))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    result = JsonConvert.DeserializeObject<Dictionary<string, DatabaseSpecification>>(Helpers.FileReadWriteHelper.ReadAllText(TinyWebApiConfigurations.ConnectionStringJSONFilePath));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (TinyWebApiConfigurations.DatabaseSpecifications is not null && TinyWebApiConfigurations.DatabaseSpecifications.Count > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    foreach (var item in TinyWebApiConfigurations.DatabaseSpecifications.Where(item => !result.ContainsKey(item.Key)))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        result.Add(item.Key, item.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        /// <summary>
        ///     Gets the mailer specifications.
        /// </summary>
        /// <value>
        ///     The mailer specifications.
        /// </value>
        [DebuggerHidden]
        public static Dictionary<string, MailerSpecification> MailerSpecifications
        {
            get
            {
                LogInformation("Returning the MailerSpecifications from JSON file or Pre-Configured code or merge of both and if not available then default.");
                Dictionary<string, MailerSpecification> result = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (File.Exists(TinyWebApiConfigurations.MailerJSONFilePath))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    result = JsonConvert.DeserializeObject<Dictionary<string, MailerSpecification>>(Helpers.FileReadWriteHelper.ReadAllText(TinyWebApiConfigurations.MailerJSONFilePath));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (TinyWebApiConfigurations.MailerSpecifications is not null && TinyWebApiConfigurations.MailerSpecifications.Count > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    foreach (var item in TinyWebApiConfigurations.MailerSpecifications.Where(item => !result.ContainsKey(item.Key)))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        result.Add(item.Key, item.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        /// <summary>
        ///     Gets the query specifications.
        ///     If queries to be segregatted in different files then place JSON only files supporting only queries configuration under ConfigurationDirectoryPath\Queries\{Environment} folder. Remember query keys should be unique across all files if not then first occurance will be picked up and remaning occurances will be skipped.
        /// </summary>
        /// <value>
        ///     The query specifications.
        /// </value>
        [DebuggerHidden]
        public static Dictionary<string, QuerySpecification> QuerySpecifications
        {
            get
            {
                LogInformation("Returning the QuerySpecifications from JSON file or Pre-Configured code or merge of both and if not available then default.");
                Dictionary<string, QuerySpecification> result = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (File.Exists(TinyWebApiConfigurations.QueriesJSONFilePath))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    result = JsonConvert.DeserializeObject<Dictionary<string, QuerySpecification>>(Helpers.FileReadWriteHelper.ReadAllText(TinyWebApiConfigurations.QueriesJSONFilePath));
                if(Directory.Exists(Path.Combine(ConfigurationDirectoryPath, "Queries", Environment)))
                {
                    if (result is null) result = new();
                    try { foreach (var r1 in from item in (FileInfo[]?)new DirectoryInfo(Path.Combine(ConfigurationDirectoryPath, "Queries", Environment)).GetFiles("*.json") let r = JsonConvert.DeserializeObject<Dictionary<string, QuerySpecification>>(Helpers.FileReadWriteHelper.ReadAllText(item.FullName)) from r1 in from r1 in r where !result.ContainsKey(r1.Key) select r1 select r1) result.Add(r1.Key, r1.Value); } catch { }
                }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (TinyWebApiConfigurations.QuerySpecifications is not null && TinyWebApiConfigurations.QuerySpecifications.Count > 0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    foreach (var item in TinyWebApiConfigurations.QuerySpecifications.Where(item => !result.ContainsKey(item.Key)))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        result.Add(item.Key, item.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
        /// <summary>
        ///     Gets database specification by database name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The database specification by database name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8603 // Possible null reference return.
        public static DatabaseSpecification GetDatabaseSpecificationByDatabaseName(string name) => DatabaseSpecifications.GetValueOrDefault(name);
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        ///     Gets mailer specification by mailer name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The mailer specification by mailer name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8603 // Possible null reference return.
        public static MailerSpecification GetMailerSpecificationByMailerName(string name) => MailerSpecifications.GetValueOrDefault(name);
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        ///     Gets run as user specification by user name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The run as user specification by user name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8603 // Possible null reference return.
        public static RunAsUserSpecification GetRunAsUserSpecificationByUserName(string name) => RunAsUserSpecifications.GetValueOrDefault(name);
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        ///     Gets query specification by query name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The query specification by query name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8603 // Possible null reference return.
        public static QuerySpecification GetQuerySpecificationByQueryName(string name) => QuerySpecifications.GetValueOrDefault(name);
#pragma warning restore CS8603 // Possible null reference return.
        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        [DebuggerHidden]
        public static IConfiguration? Configuration { get; set; }
        /// <summary>
        ///     Gets or sets the web hosting environment.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        [DebuggerHidden]
        public static IHostEnvironment? WebHostingEnvironment { get; set; }
        /// <summary>
        ///     Gets the service IP.
        /// </summary>
        /// <value>
        ///     The service IP.
        /// </value>
        [DebuggerHidden]
        public static IPAddress? ServiceIP { get; internal set; }
        /// <summary>
        ///     Gets a context for the current HTTP.
        /// </summary>
        /// <value>
        ///     The current HTTP context.
        /// </value>
        [DebuggerHidden]
        public static HttpContext? CurrentHttpContext { get; internal set; }
        /// <summary>
        ///     Gets the service port.
        /// </summary>
        /// <value>
        ///     The service port.
        /// </value>
        [DebuggerHidden]
        public static int ServicePort { get; internal set; }
        /// <summary>
        /// Gets or sets the tiny web api configurations.
        /// </summary>
        /// <value>
        ///     The tiny web api configurations.
        /// </value>
        [DebuggerHidden]
        public static ITinyWebApiConfigurations? TinyWebApiConfigurations { get; set; }
        /// <summary>
        /// Gets the key from the Query specifications based on query specification value.
        /// </summary>
        /// <param name="querySpecification"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string GetKeyFromQuerySpecificationValue(QuerySpecification querySpecification) => QuerySpecifications.FirstOrDefault(o => o.Value.Query == querySpecification.Query).Key;
    }
}
