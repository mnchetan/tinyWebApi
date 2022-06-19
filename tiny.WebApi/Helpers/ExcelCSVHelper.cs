// <copyright file="ExcelCSVHelper.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using tiny.WebApi.DataObjects;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     An excel CSV helper.
    /// </summary>
    [DebuggerStepThrough]
    public static class ExcelCSVHelper
    {
        /// <summary>
        ///     Export to excel.
        /// </summary>
        /// <param name="dt"> The dt. </param>
        /// <param name="sheetName">Provide sheet name for excel sheet  (optional).</param>
        /// <param name="isFlushDataTableOnceExported">Dispose the databale once exported.</param>
        /// <returns>
        ///     A byte[].
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] ExportToExcel(DataTable dt, string sheetName = "", bool isFlushDataTableOnceExported = false)
        {
            Global.LogDebug("Inside ExceportToExcel, exporting DataTable to ByteArray.");
            var ds = new DataSet();
            ds.Tables.Add(dt);
            var d = ExportToExcel(ds, sheetName);
            if (isFlushDataTableOnceExported && ds != null) ds.Dispose();
            return d;
        }
        /// <summary>
        ///     Export to excel.
        /// </summary>
        /// <param name="ds"> The ds. </param>
        /// <param name="sheetNameCommaSeperated">Provide sheet name(s) (comma seperated) for excel sheet  (optional).</param>
        /// <param name="isFlushDataSetOnceExported"></param>
        /// <returns>
        ///     A byte[].
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] ExportToExcel(DataSet ds, string sheetNameCommaSeperated = "", bool isFlushDataSetOnceExported = false)
        {
            Global.LogDebug("Inside ExceportToExcel, exporting DataSet to ByteArray.");
            using MemoryStream ms = new();
            using (XLWorkbook wb = new())
            {
                var arr = $"{sheetNameCommaSeperated}".Split(',');
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (arr != null && arr.Length >= i + 1 && !string.IsNullOrEmpty(arr[i])) ds.Tables[i].TableName = arr[i][0] == '_' || char.IsLetter(arr[i][0]) ? arr[i] : "cnmunderscore" + arr[i];
                    _ = string.IsNullOrWhiteSpace(ds.Tables[i].TableName) ? wb.Worksheets.Add(ds.Tables[i]) : wb.Worksheets.Add(ds.Tables[i], ds.Tables[i].TableName.Replace("cnmunderscore", ""));
                    if (isFlushDataSetOnceExported)
                        ds.Tables[i].Rows.Clear();
                }
                wb.SaveAs(ms);
            }
            Global.LogDebug("Returning ByteArray.");
            if (isFlushDataSetOnceExported && ds != null) ds.Dispose();
            return ms.ToArray();
        }
        /// <summary>
        ///     A DataTable extension method that data table as XML.
        /// </summary>
        /// <param name="dt"> The dt. </param>
        /// <returns>
        ///     A string.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static string DataTableAsXML(this DataTable dt)
        {
            Global.LogDebug("Inside DataTableAsXML, converting DataTable as XML.");
            if (dt is not null && string.IsNullOrWhiteSpace(dt.TableName)) dt.TableName = "Table";
            using StringWriter sw = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dt.WriteXml(sw);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Global.LogDebug("Returning XML string.");
            return sw.ToString();
        }
        /// <summary>
        ///     Generates an excel.
        /// </summary>
        /// <param name="dt"> The dt. </param>
        /// <returns>
        ///     An array of byte.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] GenerateExcel(DataTable dt)
        {
            Global.LogDebug("Inside GenerateExcel, converting DataTable as ByteArray.");
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return GenerateExcel(ds);
        }
        /// <summary>
        ///     Generates an excel.
        /// </summary>
        /// <param name="ds"> The ds. </param>
        /// <returns>
        ///     An array of byte.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] GenerateExcel(DataSet ds)
        {
            Global.LogDebug("Inside GenerateExcel, converting DataSet as ByteArray.");
            using MemoryStream ms = new();
            using (var workbook = SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new();
                workbook.WorkbookPart.Workbook.Sheets = new();
                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new();
                    sheetPart.Worksheet = new(sheetData);
                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationShipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Any()) sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    Sheet sheet = new() { Id = relationShipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);
                    Row headerRow = new();
                    List<string> columns = new();
                    foreach (DataColumn col in table.Columns)
                    {
                        columns.Add(col.ColumnName);
                        Cell cell = new();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new(col.ColumnName);
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);
                    foreach (DataRow dsRow in table.Rows)
                    {
                        Row newrow = new();
                        foreach (string col in columns)
                        {
                            Cell cell = new();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new(dsRow[col] + ""); ;
                            newrow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newrow);
                    }
                }
            }
            Global.LogDebug("Returning ByteArray.");
            return ms.ToArray();
        }
        /// <summary>
        ///     Import excel to data table.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="filePath">  Path of the file. </param>
        /// <param name="sheetName"> (Optional) Name of the sheet. </param>
        /// <param name="isFlushFileData">Default true, will flush the file data once the import is completed that is will dispose the byte array shared.</param>
        /// <param name="skipHeaderRow">Default false, true if header needs to be skipped.</param>
        /// <param name="startRow">Default 0, specify the start row that is skip all the preious rows</param>
        /// <param name="fileData">byte array of the file.</param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8604 // Possible null reference argument.
        public static DataTable ImportExcelToDataTable(string filePath, string sheetName = "", bool isFlushFileData = true, bool skipHeaderRow = false, int startRow = 0, byte[]? fileData = null) => ImportExcelToDataTable(fileData, sheetName, isFlushFileData, skipHeaderRow, startRow, filePath);
#pragma warning restore CS8604 // Possible null reference argument.
        /// <summary>
        ///     Import excel to data table.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="fileData">  File Data in byte array format. </param>
        /// <param name="sheetName"> (Optional) Name of the sheet. </param>
        /// <param name="isFlushFileData">Default true, will flush the file data once the import is completed that is will dispose the byte array shared.</param>
        /// <param name="skipHeaderRow">Default false, true if header needs to be skipped.</param>
        /// <param name="startRow">Default 0, specify the start row that is skip all the preious rows</param>
        /// <param name="filePath">  Path of the file. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static DataTable ImportExcelToDataTable(byte[] fileData, string sheetName = "", bool isFlushFileData = true, bool skipHeaderRow = false, int startRow = 0, string filePath = "")
        {
            try
            {
                Global.LogDebug("Inside ImportExcelToDataTable, converting excel byte array to DataTable.");
                if (fileData is not null && fileData.Length > 0)
                {
                    using XLWorkbook workBook = new(!string.IsNullOrEmpty(filePath) && File.Exists(filePath) ? File.OpenRead(filePath) : fileData is not null ? new MemoryStream(fileData) : throw new Exception("Please provide valid file path which exists at the location or the file data in byte array format."));
                    IXLWorksheet workSheet = string.IsNullOrEmpty(sheetName) ? workBook.Worksheet(1) : workBook.Worksheet(sheetName);
                    DataTable dt = new("Table");
                    bool firstRow = true;
                    int j = 0;
                    foreach (var row in workSheet.Rows())
                    {
                        if (!row.IsEmpty())
                        {
                            if (firstRow)
                            {
                                if (!skipHeaderRow)
                                    foreach (var cell in row.Cells()) dt.Columns.Add(Convert.ToString(cell.Value));
                                else
                                {
                                    int cnm = 0;
                                    foreach (var cell in row.Cells())
                                    {
                                        dt.Columns.Add($"Column{cnm + 1}");
                                        cnm++;
                                    }
                                }
                                firstRow = false;
                            }
                            else
                            {
                                if (j >= startRow)
                                {
                                    dt.Rows.Add();
                                    int i = 0;
                                    foreach (var cell in row.Cells(1, dt.Columns.Count))
                                    {
                                        dt.Rows[^1][i] = cell.DataType switch
                                        {
                                            XLDataType.DateTime => double.TryParse(Convert.ToString(cell.Value), out double v) ? DateTime.FromOADate(v) : Convert.ToString(cell.Value),
                                            XLDataType.Number => double.TryParse(Convert.ToString(cell.Value), out double v) ? v : Convert.ToString(cell.Value),
                                            XLDataType.Boolean => bool.TryParse(Convert.ToString(cell.Value), out bool v) ? v : Convert.ToString(cell.Value),
                                            XLDataType.TimeSpan => TimeSpan.TryParse(Convert.ToString(cell.Value), out TimeSpan v) ? v : Convert.ToString(cell.Value),
                                            XLDataType.Text => Convert.ToString(cell.Value),
                                            _ => default
                                        };
                                        i++;
                                    }
                                }
                            }
                            j++;
                        }
                    }
                    Global.LogDebug("Returning Excel data as DataTable.");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (isFlushFileData) fileData = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    return dt;
                }
                else
                {
                    Global.LogDebug("Returing defualt of DataTable.");
#pragma warning disable CS8603 // Possible null reference return.
                    return default;
#pragma warning restore CS8603 // Possible null reference return.
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failure reading data from excel : {Environment.NewLine} Error : {ex.Message}");
            }
        }
        /// <summary>
        ///     Import CSV to data table.
        /// </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="delimiter">Default is comma.</param>
        /// <param name="isFlushFileData">Default true, will flush the file data once the import is completed that is will dispose the byte array shared.</param>
        /// <param name="skipHeaderRow">Default false, true if header needs to be skipped.</param>
        /// <param name="startRow">Default 0, specify the start row that is skip all the preious rows</param>
        /// <param name="fileData">byte array of the file.</param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
#pragma warning disable CS8604 // Possible null reference argument.
        public static DataTable ImportCSVToDataTable(string filePath, string delimiter = ",", bool isFlushFileData = true, bool skipHeaderRow = false, int startRow = 0, byte[]? fileData = null) => ImportCSVToDataTable(fileData, delimiter, isFlushFileData, skipHeaderRow, startRow, filePath);
#pragma warning restore CS8604 // Possible null reference argument.
        /// <summary>
        ///     Import CSV to data table.
        /// </summary>
        /// <param name="fileData"> File data. </param>
        /// <param name="delimiter">Default is comma.</param>
        /// <param name="isFlushFileData">Default true, will flush the file data once the import is completed that is will dispose the byte array shared.</param>
        /// <param name="skipHeaderRow">Default false, true if header needs to be skipped.</param>
        /// <param name="startRow">Default 0, specify the start row that is skip all the preious rows</param>
        /// <param name="filePath"> File path. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static DataTable ImportCSVToDataTable(byte[] fileData, string delimiter = ",", bool isFlushFileData = true, bool skipHeaderRow = false, int startRow = 0, string filePath = "")
        {
            Global.LogDebug("Inside ImportCSVToDataTable, converting csv byte array to DataTable taking in to consideration the double quote in data.");
            if (delimiter == "," || string.IsNullOrEmpty(delimiter))
            {
                DataTable dt = new();
                using StreamReader sr = new(!string.IsNullOrEmpty(filePath) && File.Exists(filePath) ? File.OpenRead(filePath) : fileData is not null ? new MemoryStream(fileData) : throw new Exception("Please provide valid file path which exists at the location or the file data in byte array format."));
                var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
#pragma warning disable CS8604 // Possible null reference argument.
                int j = 0;
                bool isHeaderAdded = false;
                while (!sr.EndOfStream)
                {
                    if (j >= startRow)
                    {
                        if (!isHeaderAdded)
                        {
                            var tempLine = sr.ReadLine();
                            if (string.IsNullOrEmpty(tempLine)) continue;
                            var c = csvParser.Split(tempLine);
                            if (!skipHeaderRow)
                            {
                                foreach (var item in c) _ = dt.Columns.Add(item);
                                isHeaderAdded = true;
                            }
                            else
                            {
                                for (int cnm = 0; cnm < c.Length; cnm++)
                                {
                                    _ = dt.Columns.Add($"Column{cnm + 1}");
                                }
                                isHeaderAdded = true;
                            }
                            j++;
                            continue;
                        }
                        var line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            var x = csvParser.Split(line);
                            var row = dt.NewRow();
                            for (int i = 0; i < x.Length; i++)
                            {
                                if (x[i].StartsWith("\"") && x[i].EndsWith("\""))
                                {
                                    x[i] = x[i][1..];
                                    x[i] = x[i][0..^1];
                                }
                                row[i] = x[i];
                            }
                            dt.Rows.Add(row);
                        }
                        else continue;
                    }
                    else
                        sr.ReadLine();
                    j++;
                }
                Global.LogDebug("Returning csv data as DataTable.");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                if (isFlushFileData) fileData = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                return dt;
            }
            else
            {
                Global.LogDebug("Returning csv data as DataTable.");
                return ImportNonCommaDelimitedToDataTable(fileData, delimiter, isFlushFileData, skipHeaderRow, startRow, filePath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="delimiter"></param>
        /// <param name="isFlushFileData">Default is true.</param>
        /// <param name="skipHeaderRow">Default false, true if header needs to be skipped.</param>
        /// <param name="startRow">Default 0, specify the start row that is skip all the preious rows</param>
        /// <param name="filePath"> File path. </param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static DataTable ImportNonCommaDelimitedToDataTable(byte[] fileData, string delimiter, bool isFlushFileData = true, bool skipHeaderRow = false, int startRow = 0, string filePath = "")
        {
            DataTable datatable = new();
            using StreamReader streamreader = new(!string.IsNullOrEmpty(filePath) && File.Exists(filePath) ? File.OpenRead(filePath) : fileData is not null ? new MemoryStream(fileData) : throw new Exception("Please provide valid file path which exists at the location or the file data in byte array format."));

            int i = 0;
            var isHeaderAdded = false;
            while (streamreader.Peek() > 0)
            {
                if (i >= startRow)
                {
                    if (!isHeaderAdded)
                    {
                        var tempLine = streamreader.ReadLine();
                        if (string.IsNullOrEmpty(tempLine)) continue;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        string[] columnheaders = streamreader.ReadLine().Split(delimiter);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        if (!skipHeaderRow)
                        {
                            foreach (string columnheader in columnheaders)
                                datatable.Columns.Add(columnheader); // I've added the column headers here.
                            isHeaderAdded = true;
                        }
                        else
                        {
                            for (int cnm = 0; cnm < columnheaders.Length; cnm++)
                            {
                                _ = datatable.Columns.Add($"Column{cnm + 1}");
                            }
                            isHeaderAdded = true;
                        }
                        i++;
                        continue;
                    }
                    var line = streamreader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        DataRow datarow = datatable.NewRow();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        datarow.ItemArray = streamreader.ReadLine().Split(delimiter);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        datatable.Rows.Add(datarow);
                    }
                    else continue;
                }
                else streamreader.ReadLine();
                i++;
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (isFlushFileData) fileData = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            return datatable;
        }
    }
}
