// <copyright file="Exceptions.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using tiny.WebApi.DataObjects;
using tiny.WebApi.Enums;
using tiny.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static tiny.WebApi.DataObjects.Global;
namespace tiny.WebApi.Extensions
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
            LogInformation($"Inside ProcessInputData.");
            string filePath;
            LogInformation($"Return requestSpecifications as it is if PreProcessFileName or class name is empty or null.");
            if (string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PreProcessing) || string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing)) return requestSpecifications;
            if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
            {
                LogInformation($"If Process file exists then load load the assembly from the file path.");
                var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing);
                if (dll is not null)
                    try
                    {
                        LogInformation($"Call the Process Input Data method of the external assembly.");
                        return dll.ProcessInputData(key, requestSpecifications, querySpecification, ref isDoNotFireFurtherQuery, ref outPut);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, querySpecification, filePath);
                        ThrowException(ex, querySpecification, filePath);
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
            LogInformation($"Inside ProcessOutPutData.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data method of the external assembly.");
                            input = dll.ProcessOutPutData(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            LogInformation($"Inside ProcessOutPutData.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data method of the external assembly.");
                            input = dll.ProcessOutPutData(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            LogInformation($"Inside ProcessOutPutDataForExcel.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data for Excel method of the external assembly.");
                            input = dll.ProcessOutPutDataForExcel(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            var result = ExcelCSVHelper.ExportToExcel(input as DataSet);
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, result, OutPutType.Excel);
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
            LogInformation($"Inside ProcessOutPutDataForExcel.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data for Excel method of the external assembly.");
                            input = dll.ProcessOutPutDataForExcel(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            var result = ExcelCSVHelper.ExportToExcel(input as DataTable);
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, result, OutPutType.Excel);
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
            LogInformation($"Inside ProcessOutPutDataForCSV.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data for CSV method of the external assembly.");
                            input = dll.ProcessOutPutDataForCSV(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            var result = (input as DataTable).DataTableToCSV();
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, result, OutPutType.CSV);
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
            LogInformation($"Inside ProcessOutPutDataForPDF.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data for PDF method of the external assembly.");
                            input = dll.ProcessOutPutDataForPDF(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            var result = PDFHelper.ExportToPDF(input as DataSet);
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, result, OutPutType.PDF);
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
            LogInformation($"Inside ProcessOutPutDataForPDF.");
            string filePath;
            dynamic input = null;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Data for PDF method of the external assembly.");
                            input = dll.ProcessOutPutDataForPDF(key, outPut, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            DataSet ds = new();
            ds.Tables.Add(input as DataTable);
            var result = PDFHelper.ExportToPDF(ds);
            if (querySpecification.IsAllowSendingJSONInMail) MailOutPut(querySpecification, result, OutPutType.PDF);
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
            LogInformation($"Inside ProcessOutPutScalarNonScalar.");
            string filePath;
            LogInformation($"Return output data as it is if PostProcessFileName or class name is empty or null.");
            if (!string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing))
                if (File.Exists(filePath = GetProcessFilePath(querySpecification)))
                {
                    LogInformation($"If Process file exists then load load the assembly from the file path.");
                    var dll = ExternalAssemblyExecutionHelper.LoadPluginFromFile(filePath, querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing);
                    if (dll is not null)
                        try
                        {
                            LogInformation($"Call the Process Output Scalar Non Scalar method of the external assembly.");
                            dll.ProcessOutPutScalarNonScalar(key, requestSpecifications, querySpecification);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, querySpecification, filePath);
                            ThrowException(ex, querySpecification, filePath);
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
            LogInformation("Inside MailOutPut, send mail asynchronously if MailerSpecification is not null and IsSendOutputViaEmailAlso = true");
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
        private static void LogCommonWarning(QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => LogWarning($"Class {(callerName.ToLower().Contains("pre") ? querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing : querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing)} mapped to the file path {filePath} could not be found or some other issue.");
        /// <summary>
        ///     Handles the exception.
        /// </summary>
        /// <remarks>
        /// <param name="ex">                 The exception. </param>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="filePath">           Full pathname of the file. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        /// </remarks>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void HandleException(Exception ex, QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => LogError($"Some error occured while trying to execute the {callerName} mapped to the file path {filePath} of class name {(callerName.ToLower().Contains("pre") ? querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing : querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing)}. {System.Environment.NewLine} Error : {ex.Message}", ex);
        /// <summary>
        ///     Throw exception.
        /// </summary>
        /// <param name="ex">                 The exception. </param>
        /// <param name="querySpecification"> The query specification. </param>
        /// <param name="filePath">           Full pathname of the file. </param>
        /// <param name="callerName">         (Optional) Name of the caller. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private static void ThrowException(Exception ex, QuerySpecification querySpecification, string filePath, [CallerMemberName] string callerName = "") => throw new Exception($"Some error occured while trying to execute the {callerName} mapped to the file path {filePath} of class name {(callerName.ToLower().Contains("pre") ? querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing : querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing)}. {System.Environment.NewLine} Error : {ex.Message}");
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
        private static string GetProcessFilePath(QuerySpecification querySpecification, [CallerMemberName] string callerName = "") => callerName.ToLower().Contains("pre") ? !string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PreProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PreProcessing) && string.IsNullOrWhiteSpace(querySpecification.ExternalDllPathImplementingIProcessDataInterface_PreProcessing) ? Path.Combine(ConfigurationDirectoryPath, querySpecification.ExternalDllNameImplementingIProcessDataInterface_PreProcessing) : Path.Combine(querySpecification.ExternalDllPathImplementingIProcessDataInterface_PreProcessing, querySpecification.ExternalDllNameImplementingIProcessDataInterface_PreProcessing) : !string.IsNullOrWhiteSpace(querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) && !string.IsNullOrWhiteSpace(querySpecification.FullyQualifiedNameOfClassImplementingInterfaceIProcessDataInterface_PostProcessing) && string.IsNullOrWhiteSpace(querySpecification.ExternalDllPathImplementingIProcessDataInterface_PostProcessing) ? Path.Combine(ConfigurationDirectoryPath, querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing) : Path.Combine(querySpecification.ExternalDllPathImplementingIProcessDataInterface_PostProcessing, querySpecification.ExternalDllNameImplementingIProcessDataInterface_PostProcessing);
    }
}
