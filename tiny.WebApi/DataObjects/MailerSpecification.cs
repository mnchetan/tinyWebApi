// <copyright file="MailerSpecification.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the mailer specification class.
// </summary>
using Newtonsoft.Json;
using System.Diagnostics;
namespace tiny.WebApi.DataObjects
{
    /// <summary>
    ///     A mailer specification.
    /// </summary>
    [DebuggerStepThrough]
    public class MailerSpecification
    {
        /// <summary>
        ///     The run as user specification.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private RunAsUserSpecification runAsUSerSpecification;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the SMTP server.
        /// </summary>
        /// <value>
        ///     The SMTP server.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "SMTP_SERVER", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string SMTP_SERVER { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the SMTP port.
        /// </summary>
        /// <value>
        ///     The SMTP port.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "SMTP_PORT", Required = Required.Always, NullValueHandling = NullValueHandling.Include)]
        public int SMTP_PORT { get; set; }
        /// <summary>
        ///     Gets a value indicating whether this object is encrypted.
        /// </summary>
        /// <value>
        ///     True if this object is encrypted, false if not.
        /// </value>
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RunAsUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        ///     Gets the source for the.
        /// </summary>
        /// <value>
        ///     from.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "From", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string From { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets to.
        /// </summary>
        /// <value>
        ///     to.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "To", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string To { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the Cc.
        /// </summary>
        /// <value>
        ///     The Cc.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "CC", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string CC { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the Bcc.
        /// </summary>
        /// <value>
        ///     The Bcc.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "BCC", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string BCC { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the subject.
        /// </summary>
        /// <value>
        ///     The subject.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Subject", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Subject { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the body.
        /// </summary>
        /// <value>
        ///     The body.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "Body", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Body { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets the name of the attachment.
        /// </summary>
        /// <value>
        ///     The name of the attachment.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "AttachmentName", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string AttachmentName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        ///     Gets a value indicating whether this object is body HTML.
        /// </summary>
        /// <value>
        ///     True if this object is body html, false if not.
        /// </value>
        [DebuggerHidden]
        [JsonProperty(PropertyName = "IsBodyHtml", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public bool IsBodyHtml { get; set; }
    }
}
