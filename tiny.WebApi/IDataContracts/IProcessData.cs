// <copyright file="IProcessData.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Declares the IProcessData interface.
// </summary>
using tiny.WebApi.DataObjects;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
namespace tiny.WebApi.IDataContracts
{
    /// <summary>
    ///     Interface for process data.
    /// </summary>
    public interface IProcessData
    {
        /// <summary>
        ///     Process the input data.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <param name="isDoNotFireFurtherQuery">  [in,out] True if is do fire further query, false if not. </param>
        /// <param name="opuput">                [in,out] The opuput. </param>
        /// <returns>
        ///     A List&lt;RequestSpecification&gt;
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        List<RequestSpecification> ProcessInputData(string key, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification, ref bool isDoNotFireFurtherQuery, ref object opuput);
        /// <summary>
        ///     Process the out put data.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        dynamic ProcessOutPutData(string key, DataTable outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A dynamic.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        dynamic ProcessOutPutData(string key, DataSet outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data for excel.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataTable ProcessOutPutDataForExcel(string key, DataTable outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data for excel.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataSet ProcessOutPutDataForExcel(string key, DataSet outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data for PDF.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataTable ProcessOutPutDataForPDF(string key, DataTable outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data for PDF.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataSet ProcessOutPutDataForPDF(string key, DataSet outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put data for CSV.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="outPut">                The out put. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        DataTable ProcessOutPutDataForCSV(string key, DataTable outPut, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
        /// <summary>
        ///     Process the out put scalar non scalar.
        /// </summary>
        /// <param name="key">                   The key. </param>
        /// <param name="requestSpecifications"> The request specifications. </param>
        /// <param name="querySpecification">    The query specification. </param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        void ProcessOutPutScalarNonScalar(string key, List<RequestSpecification> requestSpecifications, QuerySpecification querySpecification);
    }
}
