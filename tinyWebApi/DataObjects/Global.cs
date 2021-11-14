/// <copyright file="Global.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using tinyWebApi.Configurations;
using System.Linq;
namespace tinyWebApi.Common.DataObjects
{
    /// <summary>
    ///     A global.
    /// </summary>
    [DebuggerStepThrough]
    public static class Global
    {
        /// <summary>
        ///     Gets the full path name of the configuration directory.
        ///     Fetched from tinyWebApi.Configurations.ITinyWebApiConfigurations.ConfigurationDirectoryPath and is overriden by appsettings.environment.json and if bot not specified then executing assembly path is taken up.
        /// </summary>
        /// <value>
        ///     The full path name of the configuration directory.
        /// </value>
        [DebuggerHidden]
        public static string ConfigurationDirectoryPath => !string.IsNullOrEmpty(Configuration?.GetSection("ConfigurationDirectoryPath").Value) && Directory.Exists(Configuration?.GetSection("ConfigurationDirectoryPath").Value) ? Configuration?.GetSection("AllowedCorsHosts").Value : !string.IsNullOrEmpty(TinyWebApiConfigurations.ConfigurationDirectoryPath) && Directory.Exists(TinyWebApiConfigurations.ConfigurationDirectoryPath) ? TinyWebApiConfigurations.ConfigurationDirectoryPath : new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
        /// <summary>
        ///     Logs an information.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogInformation(string message, object objToLog = null) => Logger?.LogInformation(message, objToLog);
        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="objToLog">The obj to log.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="ex">The ex.</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogCritical(string message, object objToLog = default, EventId eventId = default, Exception ex = default) => Logger?.LogCritical(eventId, ex, message, objToLog);
        /// <summary>
        ///     Logs a trace.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogTrace(string message, object objToLog = null) => Logger?.LogTrace(message, objToLog);
        /// <summary>
        ///     Logs a warning.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogWarning(string message, object objToLog = null) => Logger?.LogWarning(message, objToLog);
        /// <summary>
        ///     Logs a critical.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogCritical(string message, object objToLog = null) => Logger?.LogCritical(message, objToLog);
        /// <summary>
        ///     Logs a debug.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogDebug(string message, object objToLog = null) => Logger?.LogDebug(message, objToLog);
        /// <summary>
        ///     Logs an error.
        /// </summary>
        /// <param name="message">  The message. </param>
        /// <param name="ex">       The exception. </param>
        /// <param name="objToLog"> (Optional) The object to log. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void LogError(string message, Exception ex, object objToLog = null) => Logger?.LogError(message, ex, objToLog);
        /// <summary>
        ///     Gets the environment.
        /// </summary>
        /// <value>
        ///     The environment.
        /// </value>
        [DebuggerHidden]
        public static string Environment => string.IsNullOrEmpty(WebHostingEnvironment.EnvironmentName) ? "Development" : WebHostingEnvironment.EnvironmentName;
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
                Dictionary<string, RunAsUserSpecification> result = new();
                if (File.Exists(TinyWebApiConfigurations.RunAsUserJSONFilePath))
                    result = JsonConvert.DeserializeObject<Dictionary<string, RunAsUserSpecification>>(File.ReadAllText(TinyWebApiConfigurations.RunAsUserJSONFilePath));
                if (TinyWebApiConfigurations.RunAsUserSpecifications is not null && TinyWebApiConfigurations.RunAsUserSpecifications.Count > 0)
                    foreach (var item in TinyWebApiConfigurations.RunAsUserSpecifications.Where(item => !result.ContainsKey(item.Key)))
                        result.Add(item.Key, item.Value);
                return result;
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
                Dictionary<string, DatabaseSpecification> result = new();
                if (File.Exists(TinyWebApiConfigurations.ConnectionStringJSONFilePath))
                    result = JsonConvert.DeserializeObject<Dictionary<string, DatabaseSpecification>>(File.ReadAllText(TinyWebApiConfigurations.ConnectionStringJSONFilePath));
                if (TinyWebApiConfigurations.DatabaseSpecifications is not null && TinyWebApiConfigurations.DatabaseSpecifications.Count > 0)
                    foreach (var item in TinyWebApiConfigurations.DatabaseSpecifications.Where(item => !result.ContainsKey(item.Key)))
                        result.Add(item.Key, item.Value);
                return result;
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
                Dictionary<string, MailerSpecification> result = new();
                if (File.Exists(TinyWebApiConfigurations.MailerJSONFilePath))
                    result = JsonConvert.DeserializeObject<Dictionary<string, MailerSpecification>>(File.ReadAllText(TinyWebApiConfigurations.MailerJSONFilePath));
                if (TinyWebApiConfigurations.MailerSpecifications is not null && TinyWebApiConfigurations.MailerSpecifications.Count > 0)
                    foreach (var item in TinyWebApiConfigurations.MailerSpecifications.Where(item => !result.ContainsKey(item.Key)))
                        result.Add(item.Key, item.Value);
                return result;
            }
        }
        /// <summary>
        ///     Gets the query specifications.
        /// </summary>
        /// <value>
        ///     The query specifications.
        /// </value>
        [DebuggerHidden]
        public static Dictionary<string, QuerySpecification> QuerySpecifications
        {
            get
            {
                Dictionary<string, QuerySpecification> result = new();
                if (File.Exists(TinyWebApiConfigurations.QueriesJSONFilePath))
                    result = JsonConvert.DeserializeObject<Dictionary<string, QuerySpecification>>(File.ReadAllText(TinyWebApiConfigurations.QueriesJSONFilePath));
                if (TinyWebApiConfigurations.QuerySpecifications is not null && TinyWebApiConfigurations.QuerySpecifications.Count > 0)
                    foreach (var item in TinyWebApiConfigurations.QuerySpecifications.Where(item => !result.ContainsKey(item.Key)))
                        result.Add(item.Key, item.Value);
                return result;
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
        public static DatabaseSpecification GetDatabaseSpecificationByDatabaseName(string name) => DatabaseSpecifications.GetValueOrDefault(name);
        /// <summary>
        ///     Gets mailer specification by mailer name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The mailer specification by mailer name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static MailerSpecification GetMailerSpecificationByMailerName(string name) => MailerSpecifications.GetValueOrDefault(name);
        /// <summary>
        ///     Gets run as user specification by user name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The run as user specification by user name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static RunAsUserSpecification GetRunAsUserSpecificationByUserName(string name) => RunAsUserSpecifications.GetValueOrDefault(name);
        /// <summary>
        ///     Gets query specification by query name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The query specification by query name.
        /// </returns>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static QuerySpecification GetQuerySpecificationByQueryName(string name) => QuerySpecifications.GetValueOrDefault(name);
        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static IConfiguration? Configuration { get; set; }
        /// <summary>
        ///     Gets or sets the web hosting environment.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        [DebuggerHidden]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static IHostEnvironment? WebHostingEnvironment { get; set; }
        /// <summary>
        ///     Gets the service IP.
        /// </summary>
        /// <value>
        ///     The service IP.
        /// </value>
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        [DebuggerHidden]
        public static IPAddress ServiceIP { get; internal set; }
        /// <summary>
        ///     Gets a context for the current HTTP.
        /// </summary>
        /// <value>
        ///     The current HTTP context.
        /// </value>
        [DebuggerHidden]
        public static HttpContext CurrentHttpContext { get; internal set; }
        /// <summary>
        ///     Gets the service port.
        /// </summary>
        /// <value>
        ///     The service port.
        /// </value>
        [DebuggerHidden]
        public static int ServicePort { get; internal set; }
        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <value>
        ///     The logger.
        /// </value>
        [DebuggerHidden]
        public static ILogger Logger { get; internal set; } = LoggerFactory?.CreateLogger("StaticGenericLogger");
        /// <summary>
        /// Gets or sets the tiny web api configurations.
        /// </summary>
        /// <value>
        ///     The tiny web api configurations.
        /// </value>
        [DebuggerHidden]
        public static ITinyWebApiConfigurations TinyWebApiConfigurations { get; set; }
        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        [DebuggerHidden]
        public static ILoggerFactory LoggerFactory { get; internal set; }
    }
}
