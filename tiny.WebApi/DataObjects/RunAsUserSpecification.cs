// <copyright file="RunAsUserSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the run as user specification class.
// </summary>
using Newtonsoft.Json;
using System.Diagnostics;
namespace tiny.WebApi.DataObjects
{
    /// <summary>
    ///     A run as user specification.
    /// </summary>
    [DebuggerStepThrough]
    public class RunAsUserSpecification
    {
        /// <summary>
        ///     Gets the run as domain.
        /// </summary>
        /// <value>
        ///     The run as domain.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "RunAsDomain", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RunAsDomain { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the name of the run as user.
        /// </summary>
        /// <value>
        ///     The name of the run as user.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "RunAsUserName", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RunAsUserName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the run as password.
        /// </summary>
        /// <value>
        ///     The run as password.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "RunAsPassword", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RunAsPassword { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets a value indicating whether this object is run as password encrypted.
        /// </summary>
        /// <value>
        ///     True if this object is run as password encrypted, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsRunAsPasswordEncrypted", Required = Required.Default)]
        public bool IsRunAsPasswordEncrypted { get; set; }
        /// <summary>
        ///     Gets the encryption key.
        /// </summary>
        /// <value>
        ///     The encryption key.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "EncryptionKey", Required = Required.Default)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string EncryptionKey { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
