// <copyright file="TinyWebApiConfigurations.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     TinyWebApiConfigurations implements Itiny.WebApiConfigurations interface.
//     This is used to push the configurations necessary to inject in the DI for functionaning of the tiny.WebApi.
// </summary>
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Configurations
{
    /// <summary>
    /// The Tiny web api configurations.
    /// </summary>
    [DebuggerStepThrough]
    public class TinyWebApiConfigurations : ITinyWebApiConfigurations
    {
        /// <summary>
        /// Gets or sets the queries file name without extension.
        /// This value is must if QuerySpecifications is not specified.
        /// QueriesJSONFileNameWithoutExtension or QuerySpecifications is mandatory for application.
        /// If both provided then both configurations will be merged where in configurations in the json file will be take precedence in case of duplicates.
        /// Make sure QueriesJSONFileNameWithoutExtension.environment.json is present at ConfigurationDirectoryPath.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string QueriesJSONFileNameWithoutExtension { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets the queries JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string QueriesJSONFilePath => !string.IsNullOrEmpty(QueriesJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{QueriesJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : "." + Global.Environment)}.json") : "";
        /// <summary>
        /// Gets or sets the connection strings file name without extension.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ConnectionStringJSONFileNameWithoutExtension { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets the connection string JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string ConnectionStringJSONFilePath => !string.IsNullOrEmpty(ConnectionStringJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{ConnectionStringJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : "." + Global.Environment)}.json") : "";
        /// <summary>
        /// Gets or sets the mailers file name without extension.
        /// </summary>
        [DebuggerHidden]
        public string MailerJSONFileNameWithoutExtension { get; set; } = "";
        /// <summary>
        /// Gets the mailer JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string MailerJSONFilePath => !string.IsNullOrEmpty(MailerJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{MailerJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : $".{Global.Environment}")}.json") : "";
        /// <summary>
        /// Gets or sets the run as user file name without extension.
        /// </summary>
        [DebuggerHidden]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RunAsUserJSONFileNameWithoutExtension { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// Gets the run as user JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string RunAsUserJSONFilePath => !string.IsNullOrEmpty(RunAsUserJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{RunAsUserJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : "." + Global.Environment)}.json") : "";
        /// <summary>
        /// Gets or sets the query specifications.
        /// This value is must if QueriesJSONFileNameWithoutExtension is not specified.
        /// QueriesJSONFileNameWithoutExtension or QuerySpecifications is mandatory for application.
        /// If both provided then both configurations will be merged where in configurations in the json file will be take precedence in case of duplicates.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, QuerySpecification> QuerySpecifications { get; set; } = new();
        /// <summary>
        /// Gets or sets the mailer specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, MailerSpecification> MailerSpecifications { get; set; } = new();
        /// <summary>
        /// Gets or sets the database specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, DatabaseSpecification> DatabaseSpecifications { get; set; } = new();
        /// <summary>
        /// Gets or sets the run as user specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, RunAsUserSpecification> RunAsUserSpecifications { get; set; } = new();
        /// <summary>
        /// Gets the configuration directory path.
        /// </summary>
        [DebuggerHidden]
        public string ConfigurationDirectoryPath { get; set; } = "";
    }
}
