// <copyright file="PDFHelper.cs" company="tiny">
//     Copyright (c) 2021 tiny. All rights reserved.
// </copyright>
// <summary>
//     Implements the PDF helper class.
// </summary>
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using tiny.WebApi.DataObjects;
using tiny.WebApi.Extensions;
namespace tiny.WebApi.Helpers
{
    /// <summary>
    ///     A PDF helper.
    /// </summary>
    //[DebuggerStepThrough]
    public static class PDFHelper
    {
        /// <summary>
        ///     A DataSet extension method that export to PDF.
        /// </summary>
        /// <param name="ds"> The ds to act on. </param>
        /// <returns>
        ///     A byte[].
        /// </returns>
        //[DebuggerStepThrough]
        //[DebuggerHidden]
        public static byte[] ExportToPDF(this DataSet ds)
        {
            Global.LogDebug("Inside ExportToPDF, export the DataSet to ByteArray.");
            if (ds is null) ds = new();
            using MemoryStream ms = new();
            Document document = new();
            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();
            foreach (var (dt, table) in from DataTable dt in ds.Tables let table = new PdfPTable(dt.Columns.Count) select (dt, table))
            {
                table.WidthPercentage = 100;
                foreach (DataColumn column in dt.Columns) table.AddCell(new PdfPCell(new Phrase(column.ColumnName))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(0, 118, 182)
                });
                foreach (var (row, column) in from DataRow row in dt.Rows from DataColumn column in dt.Columns select (row, column))
                {
                    table.AddCell(new PdfPCell(new Phrase(Convert.ToString(row.GetValueOrDefault<string>(column.ColumnName))))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_CENTER
                    });
                }
                document.Add(table);
            }
            document.Close();
            return ms.ToArray();
        }
    }
}
