// <copyright file="ITinyWebApiConfigurations.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     ITinyWebApiConfigurations interface.
// </summary>
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Configurations
{
    /// <summary>
    /// The ITiny web api configurations.
    /// </summary>
    public interface ITinyWebApiConfigurations
    {
        /// <summary>
        /// Gets or sets the queries file name without extension.
        /// This value is must if QuerySpecifications is not specified.
        /// QueriesJSONFileNameWithoutExtension or QuerySpecifications is mandatory for application.
        /// If both provided then both configurations will be merged where in configurations in the json file will be take precedence in case of duplicates.
        /// Make sure QueriesJSONFileNameWithoutExtension.environment.json is present at ConfigurationDirectoryPath.
        /// </summary>
        [DebuggerHidden]
        public string QueriesJSONFileNameWithoutExtension { get; set; }
        /// <summary>
        /// Gets the queries JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string QueriesJSONFilePath => !string.IsNullOrEmpty(QueriesJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{QueriesJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : "." + Global.Environment)}.json") : "";
        /// <summary>
        /// Gets or sets the connection strings file name without extension.
        /// </summary>
        [DebuggerHidden]
        public string ConnectionStringJSONFileNameWithoutExtension { get; set; }
        /// <summary>
        /// Gets the connection string JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string ConnectionStringJSONFilePath => !string.IsNullOrEmpty(ConnectionStringJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{ConnectionStringJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : "." + Global.Environment)}.json") : "";
        /// <summary>
        /// Gets or sets the mailers file name without extension.
        /// </summary>
        [DebuggerHidden]
        public string MailerJSONFileNameWithoutExtension { get; set; }
        /// <summary>
        /// Gets the mailer JSON file path.
        /// </summary>
        [DebuggerHidden]
        public string MailerJSONFilePath => !string.IsNullOrEmpty(MailerJSONFileNameWithoutExtension) ? Path.Combine(Global.ConfigurationDirectoryPath, $"{MailerJSONFileNameWithoutExtension}{(string.IsNullOrEmpty(Global.Environment) ? "" : $".{Global.Environment}")}.json") : "";
        /// <summary>
        /// Gets or sets the run as user file name without extension.
        /// </summary>
        [DebuggerHidden]
        public string RunAsUserJSONFileNameWithoutExtension { get; set; }
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
        public Dictionary<string, QuerySpecification> QuerySpecifications { get; set; }
        /// <summary>
        /// Gets or sets the mailer specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, MailerSpecification> MailerSpecifications { get; set; }
        /// <summary>
        /// Gets or sets the database specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, DatabaseSpecification> DatabaseSpecifications { get; set; }
        /// <summary>
        /// Gets or sets the run as user specifications.
        /// </summary>
        [DebuggerHidden]
        public Dictionary<string, RunAsUserSpecification> RunAsUserSpecifications { get; set; }
        /// <summary>
        /// Gets the configuration directory path.
        /// </summary>
        [DebuggerHidden]
        public string ConfigurationDirectoryPath { get; set; }
    }
}
