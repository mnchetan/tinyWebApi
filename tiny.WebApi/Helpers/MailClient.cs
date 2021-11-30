// <copyright file="MailClient.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the mail client class.
// </summary>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     A mail client.
    /// </summary>
    [DebuggerStepThrough]
    public static class MailClient
    {
        /// <summary>
        ///     (Immutable) the no reply.
        /// </summary>
        private const string NoReply = "noreply@noreply.com";
        /// <summary>
        ///     (Immutable) the CSV extension.
        /// </summary>
        internal const string CSVExtension = ".csv";
        /// <summary>
        ///     (Immutable) the JSON extension.
        /// </summary>
        internal const string JSONExtension = ".json";
        /// <summary>
        ///     (Immutable) the PDF extension.
        /// </summary>
        internal const string PDFExtension = ".pdf";
        /// <summary>
        ///     (Immutable) the excel extension.
        /// </summary>
        internal const string ExcelExtension = ".xlsx";
        /// <summary>
        ///     (Immutable) the text CSV.
        /// </summary>
        internal const string TextCSV = "text/csv";
        /// <summary>
        ///     (Immutable) the text JSON.
        /// </summary>
        internal const string TextJSON = "text/json";
        /// <summary>
        ///     (Immutable) the application PDF.
        /// </summary>
        internal const string ApplicationPDF = "application/pdf";
        /// <summary>
        ///     (Immutable) the application excel.
        /// </summary>
        internal const string ApplicationExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        /// <summary>
        ///     Sends a mail.
        /// </summary>
        /// <param name="from">        The From. </param>
        /// <param name="to">          The To. </param>
        /// <param name="cc">          (Optional) The Cc. </param>
        /// <param name="bcc">         (Optional) The Bcc. </param>
        /// <param name="subject">     The subject. </param>
        /// <param name="body">        The body. </param>
        /// <param name="isBodyHtml">  True if is body html, false if not. </param>
        /// <param name="smtp_server"> The SMTP server. </param>
        /// <param name="smtp_port">   The SMTP port. </param>
        /// <param name="content">     (Optional) The content. </param>
        /// <param name="type">        (Optional) The type. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void SendMail(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml, string smtp_server, int smtp_port, Stream content = null, ContentType type = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            Global.LogInformation("Inside SendMail, send mail asynchronously.");
            try
            {
                if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body)) return;
                using SmtpClient client = new(smtp_server, smtp_port);
                client.UseDefaultCredentials = true;
                MailAddressCollection mac = new();
                using MailMessage message = new();
                var tos = $"{to}".Split(';');
                var ccs = $"{cc}".Split(';');
                var bccs = $"{bcc}".Split(';');
                foreach (var mailto in tos.Where(mailto => !string.IsNullOrWhiteSpace(mailto))) message.To.Add(mailto.Replace(";", ""));
                foreach (var mailcc in tos.Where(mailcc => !string.IsNullOrWhiteSpace(mailcc))) message.CC.Add(mailcc.Replace(";", ""));
                foreach (var mailbcc in tos.Where(mailbcc => !string.IsNullOrWhiteSpace(mailbcc))) message.Bcc.Add(mailbcc.Replace(";", ""));
                if (string.IsNullOrWhiteSpace(from)) from = NoReply;
                message.From = new MailAddress(from);
                message.Body = $"{body}";
                message.IsBodyHtml = isBodyHtml;
                message.Subject = $"{subject}";
                if (content is not null) message.Attachments.Add(new Attachment(content, type.Name, type.MediaType));
                client.SendMailAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Global.LogError($"Unable to send mail. Check error for details. {Environment.NewLine} Error : {ex.Message}", ex);
            }
        }
        /// <summary>
        ///     Sends a mail impersonated.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="content">            The content. </param>
        /// <param name="type">               The type. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void SendMailImpersonated(QuerySpecification querySpecification, Stream content, ContentType type) => ImpersonationHelper.Execute(() => SendMail(querySpecification.MailerSpecification.From, querySpecification.MailerSpecification.To, querySpecification.MailerSpecification.CC, querySpecification.MailerSpecification.BCC, querySpecification.MailerSpecification.Subject, querySpecification.MailerSpecification.Body, querySpecification.MailerSpecification.IsBodyHtml, querySpecification.MailerSpecification.SMTP_SERVER, querySpecification.MailerSpecification.SMTP_PORT, content, type), querySpecification.MailerSpecification.RunAsUserSpecification.RunAsUserName, querySpecification.MailerSpecification.RunAsUserSpecification.RunAsDomain, querySpecification.MailerSpecification.RunAsUserSpecification.IsRunAsPasswordEncrypted ? EncryptFactory.Decrypt(querySpecification.MailerSpecification.RunAsUserSpecification.RunAsPassword, querySpecification.MailerSpecification.RunAsUserSpecification.EncryptionKey) : querySpecification.MailerSpecification.RunAsUserSpecification.RunAsPassword);
        /// <summary>
        ///     Sends a mail.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="content">            The content. </param>
        /// <param name="type">               The type. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void SendMail(QuerySpecification querySpecification, Stream content, ContentType type) => SendMail(querySpecification.MailerSpecification.From, querySpecification.MailerSpecification.To, querySpecification.MailerSpecification.CC, querySpecification.MailerSpecification.BCC, querySpecification.MailerSpecification.Subject, querySpecification.MailerSpecification.Body, querySpecification.MailerSpecification.IsBodyHtml, querySpecification.MailerSpecification.SMTP_SERVER, querySpecification.MailerSpecification.SMTP_PORT, content, type);
        /// <summary>
        ///     Sends a mail impersonated.
        /// </summary>
        /// <param name="from">          The From. </param>
        /// <param name="to">            The To. </param>
        /// <param name="cc">            (Optional) The Cc. </param>
        /// <param name="bcc">           (Optional) The Bcc. </param>
        /// <param name="subject">       The subject. </param>
        /// <param name="body">          The body. </param>
        /// <param name="isBodyHtml">    True if is body html, false if not. </param>
        /// <param name="smtp_server">   The SMTP server. </param>
        /// <param name="smtp_port">     The SMTP port. </param>
        /// <param name="runAsUserName"> Name of the run as user. </param>
        /// <param name="runAsDomain">   The run as domain. </param>
        /// <param name="runAsPassword"> The run as password. </param>
        /// <param name="content">       (Optional) The content. </param>
        /// <param name="type">          (Optional) The type. </param>
        [DebuggerHidden]
        [DebuggerStepThrough]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void SendMailImpersonated(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml, string smtp_server, int smtp_port, string runAsUserName, string runAsDomain, string runAsPassword, Stream content = null, ContentType type = null) => ImpersonationHelper.Execute(() => SendMail(from, to, cc, bcc, subject, body, isBodyHtml, smtp_server, smtp_port, content, type), runAsUserName, runAsDomain, runAsPassword);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Sends a mail impersonated.
        /// </summary>
        /// <param name="from">                The From. </param>
        /// <param name="to">                  The To. </param>
        /// <param name="cc">                  (Optional) The Cc. </param>
        /// <param name="bcc">                 (Optional) The Bcc. </param>
        /// <param name="subject">             The subject. </param>
        /// <param name="body">                The body. </param>
        /// <param name="isBodyHtml">          True if is body html, false if not. </param>
        /// <param name="mailerSpecification"> The mailer specification. </param>
        /// <param name="content">             (Optional) The content. </param>
        /// <param name="type">                (Optional) The type. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void SendMailImpersonated(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml, MailerSpecification mailerSpecification, Stream content = null, ContentType type = null) => ImpersonationHelper.Execute(() => SendMail(from, to, cc, bcc, subject, body, isBodyHtml, mailerSpecification.SMTP_SERVER, mailerSpecification.SMTP_PORT, content, type), mailerSpecification.RunAsUserSpecification.RunAsUserName, mailerSpecification.RunAsUserSpecification.RunAsDomain, mailerSpecification.RunAsUserSpecification.IsRunAsPasswordEncrypted ? EncryptFactory.Decrypt(mailerSpecification.RunAsUserSpecification.RunAsPassword, mailerSpecification.RunAsUserSpecification.EncryptionKey) : mailerSpecification.RunAsUserSpecification.RunAsPassword);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>
        ///     Sends a mail.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="content">            The content. </param>
        /// <param name="outPutType">         (Optional) Type of the out put. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void SendMail(QuerySpecification querySpecification, Stream content, OutPutType outPutType = OutPutType.JSON)
        {
            if (querySpecification.MailerSpecification.IsImpersonationNeeded) SendMailImpersonated(querySpecification, content, outPutType == OutPutType.CSV ? new ContentType(TextCSV) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + CSVExtension } : outPutType == OutPutType.Excel ? new ContentType(ApplicationExcel) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + ExcelExtension } : outPutType == OutPutType.PDF ? new ContentType(ApplicationPDF) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + PDFExtension } : new ContentType(TextJSON) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + JSONExtension });
            else SendMail(querySpecification, content, outPutType == OutPutType.CSV ? new ContentType(TextCSV) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + CSVExtension } : outPutType == OutPutType.Excel ? new ContentType(ApplicationExcel) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + ExcelExtension } : outPutType == OutPutType.PDF ? new ContentType(ApplicationPDF) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + PDFExtension } : new ContentType(TextJSON) { Name = querySpecification.MailerSpecification.AttachmentName.ResolveValues() + JSONExtension });
        }
        /// <summary>
        ///     A string extension method that resolve values and formats the date time.
        /// </summary>
        /// <param name="value"> The value to act on. </param>
        /// <returns>
        ///     A string.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string ResolveValues(this string value)
        {
            Global.LogInformation("Inside ResolveValues, resolving mail template for $ seperated date time formats.");
#pragma warning disable CS8603 // Possible null reference return.
            if (value is null) return value;
#pragma warning restore CS8603 // Possible null reference return.
            var v = value;
            var list = new List<string>();
            foreach (var s in from char v1 in v let s = v.IndexOf("$") select s)
            {
                if (s == -1) break;
                var t = v.Substring(s, v.IndexOf("$", s + 1) - s + 1);
                list.Add(t);
                v = v.Replace(t, "");
            }
            if (list is not null && list.Count > 0)
                foreach (var item in list)
                    try
                    {
                        value = value.Replace(item, DateTime.UtcNow.ToString(item.Replace("$", "")));
                    }
                    catch { }
            return value;
        }
    }
}
