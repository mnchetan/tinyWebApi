/// <copyright file="Exceptions.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
using tinyWebApi.Common.DataObjects;
using tinyWebApi.Common.Enums;
using tinyWebApi.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static tinyWebApi.Common.DataObjects.Global;
namespace tinyWebApi.Common.Extensions
{
    /// <summary>
    ///     The process data extensions.
    /// </summary>
    [DebuggerStepThrough]
    public static class ProcessDataExtensions
    {
        /// <summary>
        ///     A List&lt;RequestSpecification&gt; extension method that process the input data.
        /// </summary>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="isDoNotFireFurtherQuery">  [in,out] True if is dofire further query, false if not. </param>
        /// <param name="outPut">                [in,out] The out put. </param>
        /// <returns>
        ///     A List&lt;RequestSpecification&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static List<RequestSpecification> ProcessInputData(this List<RequestSpecification> requestSpecifications, string key, QuerySpecification querySpecification, ref bool isDoNotFireFurtherQuery, ref object outPut)
        {
            string filePath;
            if (string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) || string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName)) return requestSpecifications;
            if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
            {
                var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                if (dll is not null)
                    try
                    {
                        return dll.ProcessInputData(key, requestSpecifications, querySpecification, ref isDoNotFireFurtherQuery, ref outPut);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                    }
                LogCommonWarning(querySpecification, filePath);
            }
            else
            {
                LogWarning($"dll does not exists : {filePath}");
            }
            return requestSpecifications;
        }
        /// <summary>
        ///     A DataTable extension method that process the out put data.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutData(this DataTable outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutData(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.JSON);
            return result;
        }
        /// <summary>
        ///     A DataTable extension method that process the out put data.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutData(this DataSet outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutData(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.JSON);
            return result;
        }
        /// <summary>
        ///     A DataSet extension method that process the out put data for excel.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutDataForExcel(this DataSet outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutDataForExcel(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.Excel);
            return result;
        }
        /// <summary>
        ///     A DataSet extension method that process the out put data for excel.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutDataForExcel(this DataTable outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutDataForExcel(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.Excel);
            return result;
        }
        /// <summary>
        ///     A DataTable extension method that process the out put data for CSV.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutDataForCSV(this DataTable outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutDataForCSV(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.CSV);
            return result;
        }
        /// <summary>
        ///     A DataSet extension method that process the out put data for PDF.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutDataForPDF(this DataSet outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutDataForPDF(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.PDF);
            return result;
        }
        /// <summary>
        ///     A DataSet extension method that process the out put data for PDF.
        /// </summary>
        /// <param name="outPut">                The out put. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutDataForPDF(this DataTable outPut, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            dynamic input = null;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            input = dll.ProcessOutPutDataForPDF(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                    {
                        LogCommonWarning(querySpecification, filePath);
                        input = outPut;
                    }
                }
                else
                {
                    LogWarning($"dll does not exists : {filePath}");
                    input = outPut;
                }
            if (input is null) input = outPut;
            var result = (input as object).ToJSON();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, Encoding.ASCII.GetBytes(result), OutPutType.PDF);
            return result;
        }
        /// <summary>
        ///     Process the out put scalar non scalar.
        /// </summary>
        /// <param name="input">                 The input. </param>
        /// <param name="key">                   The key. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="requestSpecifications"> The requestSpecifications to act on. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static dynamic ProcessOutPutScalarNonScalar(object input, string key, QuerySpecification querySpecification, List<RequestSpecification> requestSpecifications)
        {
            string filePath;
            if (!string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.PreProcessClassName);
                    if (dll is not null)
                        try
                        {
                            dll.ProcessOutPutScalarNonScalar(key, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath); ThrowException(ex, querySpecification, filePath);
                        }
                    else
                        LogCommonWarning(querySpecification, filePath);
                }
                else
                    LogWarning($"dll does not exists : {filePath}");
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, null, OutPutType.PDF);
            return input;
        }
        /// <summary>
        ///     Mail out put.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="outPut">             The out put. </param>
        /// <param name="outPutType">         Type of the out put. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void MailOutPut(QuerySpecification querySpecification, dynamic outPut, OutPutType outPutType)
        {
            if (querySpecification.MailerSpecification is not null && querySpecification.IsSendOutputViaEmailAlso)
                try
                {
                    _ = Task.Run(() => MailClient.SendMail(querySpecification, outPut is null ? null : new MemoryStream(outPut), outPutType)).ConfigureAwait(false);
                }
                catch { }
        }
        /// <summary>
        ///     Logs common warning.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="filePath">           Full pathname of the file. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void LogCommonWarning(QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => LogWarning($"Class {(callerName.ToLower().Contains("pre") ? querySpecification.PreProcessClassName : querySpecification.PostProcessClassName)} mapped to the file path {filePath} could not be found or some other issue.");
        /// <summary>
        ///     Handles the exception.
        /// </summary>
        ///
        /// <remarks>
        
        
        ///
        /// <param name="ex">                 The exception. </param>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="filePath">           Full pathname of the file. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void HandleException(Exception ex, QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => LogError($"Some error occured while trying to execute the {callerName} mapped to the file path {filePath} of class name {(callerName.ToLower().Contains("pre") ? querySpecification.PreProcessClassName : querySpecification.PostProcessClassName)}. {System.Environment.NewLine} Error : {ex.Message}", ex);
        /// <summary>
        ///     Throw exception.
        /// </summary>
        /// <param name="ex">                 The exception. </param>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="filePath">           Full pathname of the file. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void ThrowException(Exception ex, QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => throw new Exception($"Some error occured while trying to execute the {callerName} mapped to the file path {filePath} of class name {(callerName.ToLower().Contains("pre") ? querySpecification.PreProcessClassName : querySpecification.PostProcessClassName)}. {System.Environment.NewLine} Error : {ex.Message}");
        /// <summary>
        ///     Gets process file path.
        /// </summary>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        ///
        /// <returns>
        ///     The process file path.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static string GetProcessFilePath(QuerySpecification querySpecification, [CallerMemberName] string callerName = "") => callerName.ToLower().Contains("pre") ? !string.IsNullOrWhiteSpace(querySpecification.PreProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PreProcessClassName) && string.IsNullOrWhiteSpace(querySpecification.PreProcessFilePath) ? Path.Combine(ConfigurationDirectoryPath, querySpecification.PreProcessFileName) : Path.Combine(querySpecification.PreProcessFilePath, querySpecification.PreProcessFileName) : !string.IsNullOrWhiteSpace(querySpecification.PostProcessFileName) && !string.IsNullOrWhiteSpace(querySpecification.PostProcessClassName) && string.IsNullOrWhiteSpace(querySpecification.PostProcessFilePath) ? Path.Combine(ConfigurationDirectoryPath, querySpecification.PostProcessFileName) : Path.Combine(querySpecification.PostProcessFilePath, querySpecification.PostProcessFileName);
    }
}
