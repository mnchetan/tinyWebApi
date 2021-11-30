// <copyright file="DatabaseSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the database specification class.
// </summary>
using tiny.WebApi.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
namespace tiny.WebApi.DataObjects
{
    /// <summary>
    ///     A database specification.
    /// </summary>
    [DebuggerStepThrough]
    public class DatabaseSpecification
    {
        /// <summary>
        ///     The run as user specification.
        /// </summary>
        private RunAsUserSpecification runAsUSerSpecification;
        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <value>
        ///     The connection string.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ConnectionString", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets a value indicating whether is encrypted.
        /// </summary>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsEncrypted", Required = Required.Default)]
        public bool IsEncrypted { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is impersonation needed.
        /// </summary>
        /// <value>
        ///     True if this object is impersonation needed, false if not.
        ///     For Linux impersonation if not ran with root access then KeyTab is needed.
        ///     Impersonation for Linux without KetTab and with root access will not work with Anonymous Authenitcation.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsImpersonationNeeded", Required = Required.Default)]
        public bool IsImpersonationNeeded { get; set; }
        /// <summary>
        ///     Gets the run as user.
        /// </summary>
        /// <value>
        ///     The run as user.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "RunAsUser", Required = Required.Default)]
        public string RunAsUser { get; set; }
        /// <summary>
        ///     Gets the run as user specification.
        /// </summary>
        /// <value>
        ///     The run as user specification.
        /// </value>
        [DebuggerHidden]
        public RunAsUserSpecification RunAsUserSpecification
        {
            get
            {
                if (runAsUSerSpecification is null) runAsUSerSpecification = Global.GetRunAsUserSpecificationByUserName(RunAsUser);
                return runAsUSerSpecification;
            }
        }
        /// <summary>
        ///     Gets the encryption key.
        ///     Encryption key should be base 64 supported string only.
        /// </summary>
        /// <value>
        ///     The encryption key.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "EncryptionKey", Required = Required.Default)]
        public string EncryptionKey { get; set; }
        /// <summary>
        ///     Gets the connection time out.
        /// </summary>
        /// <value>
        ///     The connection time out.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "ConnectionTimeOut", Required = Required.Default)]
        public int ConnectionTimeOut { get; set; } = 1200;
        /// <summary>
        ///     Gets the type of the database.
        /// </summary>
        /// <value>
        ///     The type of the database.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "DatabaseType", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseType DatabaseType { get; set; }
    }
}
