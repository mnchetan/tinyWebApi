/// <copyright file="QuerySpecification.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
/// <summary>
///     Implements the query specification class.
/// </summary>
using tinyWebApi.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
namespace tinyWebApi.Common.DataObjects
{
    /// <summary>
    ///     A query specification.
    /// </summary>
    [DebuggerStepThrough]
    public class QuerySpecification
    {
        /// <summary>
        ///     The database specification.
        /// </summary>
        private DatabaseSpecification databaseSpecification;
        /// <summary>
        ///     The mailer specification.
        /// </summary>
        private MailerSpecification mailerSpecification;
        /// <summary>
        ///     Gets the query name.
        /// </summary>
        /// <value>
        ///     The query.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Query", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        public string Query { get; set; }
        /// <summary>
        ///     Gets the type of the execution.
        /// </summary>
        /// <value>
        ///     The type of the execution.
        /// </value>
        [DebuggerHidden]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "ExecutionType", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        public ExecutionType ExecutionType { get; set; }
        /// <summary>
        ///     Gets the inputs.
        /// </summary>
        /// <value>
        ///     The inputs.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "InputFieldNamesInSequence_UDTDollarSeperatedByType", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public string Inputs { get; set; }
        /// <summary>
        ///     Gets the outputs.
        /// </summary>
        ///
        /// <value>
        ///     The outputs.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Outputs_RefCursor_InSequence_ColonSeperated_NotRequiredForMSSQL", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public string Outputs { get; set; }
        /// <summary>
        ///     Gets the database.
        /// </summary>
        /// <value>
        ///     The database.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Database", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        public string Database { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is map udt as JSON.
        /// </summary>
        /// <value>
        ///     True if this object is map udt as json, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsMapUDTAsJSON_ApplicableForOracle", Required = Required.Default)]
        public bool IsMapUDTAsJSON { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is map udt as XML.
        /// </summary>
        /// <value>
        ///     True if this object is map udt as xml, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsMapUDTAsXML_ApplicableForOracle", Required = Required.Default)]
        public bool IsMapUDTAsXML { get; set; }
        /// <summary>
        ///     Gets the full pathname of the pre process file default is the configuration path.
        /// </summary>
        /// <value>
        ///     The full pathname of the pre process file.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ExternalDllPathImplementingIProcessDataInterface_PreProcessing", Required = Required.Default)]
        public string PreProcessFilePath { get; set; }
        /// <summary>
        ///     Gets the filename of the pre process file.
        /// </summary>
        /// <value>
        ///     The filename of the pre process file.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ExternalDllNameImplementingIProcessDataInterface_PreProcessing", Required = Required.Default)]
        public string PreProcessFileName { get; set; }
        /// <summary>
        ///     Gets the name of the pre process class.
        /// </summary>
        /// <value>
        ///     The name of the pre process class.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing", Required = Required.Default)]
        public string PreProcessClassName { get; set; }
        /// <summary>
        ///     Gets the full pathname of the post process file default is the configuration path.
        /// </summary>
        /// <value>
        ///     The full pathname of the post process file.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ExternalDllPathImplementingIProcessDataInterface_PostProcessing", Required = Required.Default)]
        public string PostProcessFilePath { get; set; }
        /// <summary>
        ///     Gets the filename of the post process file.
        /// </summary>
        /// <value>
        ///     The filename of the post process file.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ExternalDllNameImplementingIProcessDataInterface_PostProcessing", Required = Required.Default)]
        public string PostProcessFileName { get; set; }
        /// <summary>
        ///     Gets the name of the post process class.
        /// </summary>
        /// <value>
        ///     The name of the post process class.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing", Required = Required.Default)]
        public string PostProcessClassName { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is send output via email also.
        /// </summary>
        /// <value>
        ///     True if this object is send output via email also, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsSendOutputViaEmailAlso", Required = Required.Default)]
        public bool IsSendOutputViaEmailAlso { get; set; }
        /// <summary>
        ///     Gets the mailer.
        /// </summary>
        /// <value>
        ///     The mailer.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Mailer", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public string Mailer { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is allow sending JSON in mail.
        /// </summary>
        /// <value>
        ///     True if this object is allow sending JSON in mail, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsAllowSendingJSONInMail", Required = Required.Default)]
        public bool IsAllowSendingJSONInMail { get; set; }
        /// <summary>
        ///     Gets the database specification.
        /// </summary>
        /// <value>
        ///     The database specification.
        /// </value>
        [DebuggerHidden]
        public DatabaseSpecification DatabaseSpecification
        {
            get
            {
                if (databaseSpecification is null) databaseSpecification = Global.GetDatabaseSpecificationByDatabaseName(Database);
                return databaseSpecification;
            }
        }
        /// <summary>
        ///     Gets the mailer specification.
        /// </summary>
        /// <value>
        ///     The mailer specification.
        /// </value>
        [DebuggerHidden]
        public MailerSpecification MailerSpecification
        {
            get
            {
                if (mailerSpecification is null) mailerSpecification = Global.GetMailerSpecificationByMailerName(Mailer);
                return mailerSpecification;
            }
        }
        /// <summary>
        ///     Gets or Sets whether the caching is required or not.
        /// </summary>
        /// <value>
        ///     The IsCachingRequired.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsCachingRequired", Required = Required.Default)]
        public bool IsCachingRequired { get; set; } = false;
        /// <summary>
        ///     Gets or Sets the duration for which the data cache to be cached and after which will be renewed.
        ///     Note: Renewal will be done on subsequent calls.
        /// </summary>
        /// <value>
        ///     The CacheDurationInSeconds.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "CacheDurationInSeconds", Required = Required.Default)]
        public int CacheDurationInSeconds { get; set; } = 0;
    }
}
