/// <copyright file="ExcelCSVHelper.cs" company="tiny">
///     Copyright (c) 2021 tiny. All rights reserved.
/// </copyright>
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
using tinyWebApi.Common.DataObjects;

namespace tinyWebApi.Common.Helpers
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
        /// <returns>
        ///     A byte[].
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] ExportToExcel(DataTable dt)
        {
            Global.LogInformation("Inside ExceportToExcel, exporting DataTable to ByteArray.");
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ExportToExcel(ds);
        }
        /// <summary>
        ///     Export to excel.
        /// </summary>
        /// <param name="ds"> The ds. </param>
        /// <returns>
        ///     A byte[].
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static byte[] ExportToExcel(DataSet ds)
        {
            Global.LogInformation("Inside ExceportToExcel, exporting DataSet to ByteArray.");
            using MemoryStream ms = new();
            using (XLWorkbook wb = new())
            {
                foreach (DataTable dt in ds.Tables) _ = string.IsNullOrWhiteSpace(dt.TableName) ? wb.Worksheets.Add(dt) : wb.Worksheets.Add(dt, dt.TableName);
                wb.SaveAs(ms);
            }
            Global.LogInformation("Returning ByteArray.");
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
            Global.LogInformation("Inside DataTableAsXML, converting DataTable as XML.");
            if (dt is not null && string.IsNullOrWhiteSpace(dt.TableName)) dt.TableName = "Table";
            using StringWriter sw = new();
            dt.WriteXml(sw);
            Global.LogInformation("Returning XML string.");
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
            Global.LogInformation("Inside GenerateExcel, converting DataTable as ByteArray.");
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
            Global.LogInformation("Inside GenerateExcel, converting DataSet as ByteArray.");
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
            Global.LogInformation("Returning ByteArray.");
            return ms.ToArray();
        }
        /// <summary>
        ///     Import excel to data table.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="fileData">  Information describing the file. </param>
        /// <param name="sheetName"> (Optional) Name of the sheet. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static DataTable ImportExcelToDataTable(byte[] fileData, string sheetName = "")
        {
            try
            {
                Global.LogInformation("Inside ImportExcelToDataTable, converting excel byte array to DataTable.");
                if (fileData is not null && fileData.Length > 0)
                {
                    using XLWorkbook workBook = new(new MemoryStream(fileData));
                    IXLWorksheet workSheet = string.IsNullOrEmpty(sheetName) ? workBook.Worksheet(1) : workBook.Worksheet(sheetName);
                    DataTable dt = new("Table");
                    bool firstRow = true;
                    foreach (var row in workSheet.Rows())
                    {
                        if (firstRow) foreach (var cell in row.Cells()) dt.Columns.Add(Convert.ToString(cell.Value));
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (var cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                            {
                                dt.Rows[^1][i] = cell.DataType switch
                                {
                                    XLDataType.DateTime => double.TryParse(Convert.ToString(cell.Value), out double v) ? DateTime.FromOADate(v) : Convert.ToString(cell.Value),
                                    XLDataType.Number => double.TryParse(Convert.ToString(cell.Value), out double v) ? v : Convert.ToString(cell.Value),
                                    XLDataType.Boolean => bool.TryParse(Convert.ToString(cell.Value), out Boolean v) ? v : Convert.ToString(cell.Value),
                                    XLDataType.TimeSpan => TimeSpan.TryParse(Convert.ToString(cell.Value), out TimeSpan v) ? v : Convert.ToString(cell.Value),
                                    XLDataType.Text => Convert.ToString(cell.Value),
                                    _ => default
                                };
                                i++;
                            }
                        }
                    }
                    Global.LogInformation("Returning Excel data as DataTable.");
                    return dt;
                }
                else
                {
                    Global.LogInformation("Returing defualt of DataTable.");
                    return default;
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
        /// <param name="fileData"> Information describing the file. </param>
        /// <returns>
        ///     A DataTable.
        /// </returns>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static DataTable ImportCSVToDataTable(byte[] fileData)
        {
            Global.LogInformation("Inside ImportCSVToDataTable, converting csv byte array to DataTable taking in to consideration the double quote in data.");
            DataTable dt = new();
            StreamReader sr = new(new MemoryStream(fileData));
            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            var c = csvParser.Split(sr.ReadLine());
            foreach (var item in c) dt.Columns.Add(item);
            while (!sr.EndOfStream)
            {
                string[] X = csvParser.Split(sr.ReadLine());
                var row = dt.NewRow();
                for (int i = 0; i < X.Length; i++)
                {
                    if (X[i].StartsWith("\"") && X[i].EndsWith("\""))
                    {
                        X[i] = X[i][1..];
                        X[i] = X[i][0..^1];
                    }
                    row[i] = X[i];
                }
                dt.Rows.Add(row);
            }
            Global.LogInformation("Returning csv data as DataTable.");
            return dt;
        }
    }
}
