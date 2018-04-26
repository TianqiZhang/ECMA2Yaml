﻿using ECMA2Yaml.Models;
using ECMA2Yaml.UndocumentedApi.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMA2Yaml.UndocumentedApi
{
    public class ReportGenerator
    {
        public static void GenerateReport(ECMAStore store, string reportFilePath)
        {
            List<ReportItem> items = new List<ReportItem>();
            items.AddRange(store.Namespaces.Values.Where(ns => ns.Uid != null).Select(ns => ValidateItem(ns)));
            items.AddRange(store.TypesByUid.Values.Select(t => ValidateItem(t)));
            items.AddRange(store.MembersByUid.Values.Select(m => ValidateItem(m)));
            items.Sort(new ReportItemComparer());

            var report = new Report()
            {
                ReportItems = items
            };

            SaveToExcel(report, reportFilePath);
        }

        private static void SaveToExcel(Report report, string reportFilePath)
        {
            var folder = Path.GetDirectoryName(reportFilePath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var p = new ExcelPackage())
            {
                GenerateSummarySheet(report, p);
                GenerateDetailsSheet(report, p);
                p.SaveAs(new FileInfo(reportFilePath));
            }
        }

        private static void GenerateSummarySheet(Report report, ExcelPackage pack)
        {
            var ws = pack.Workbook.Worksheets.Add("Summary");
            //ws.Cells["A1"].Value = "Repository:";
            //ws.Cells["B1"].Value = report.Repository;
            //ws.Cells["A1"].AutoFitColumns();
            //ws.Cells["A2"].Value = "Branch:";
            //ws.Cells["B2"].Value = report.Branch;

            ws.Cells[4, 1].Value = "Fields";
            ws.Cells[4, 2].Value = "Total Expected";
            ws.Cells[4, 3].Value = ValidationResult.Present.ToString();
            ws.Cells[4, 4].Value = ValidationResult.Missing.ToString();
            ws.Cells[4, 5].Value = ValidationResult.UnderDoc.ToString();

            var row = 5;
            foreach(FieldType fieldType in new[] { FieldType.Summary, FieldType.Parameters, FieldType.TypeParameters, FieldType.ReturnValue})
            {
                ws.Cells[row, 1].Value = fieldType.ToString();
                int total = 0;
                int totalPresent = 0;
                int totalMissing = 0;
                int totalUnderDoc = 0;
                foreach(var item in report.ReportItems)
                {
                    if (item.Results.ContainsKey(fieldType) && item.Results[fieldType] != ValidationResult.NA)
                    {
                        total++;
                        switch(item.Results[fieldType])
                        {
                            case ValidationResult.Present:
                                totalPresent++;
                                break;
                            case ValidationResult.Missing:
                                totalMissing++;
                                break;
                            case ValidationResult.UnderDoc:
                                totalUnderDoc++;
                                break;
                        }
                    }
                }
                ws.Cells[row, 2].Value = total;
                ws.Cells[row, 3].Value = totalPresent;
                ws.Cells[row, 4].Value = totalMissing;
                ws.Cells[row, 5].Value = totalUnderDoc;
                var rule = ws.ConditionalFormatting.AddDatabar(ws.Cells[row, 2, row, 5], Color.SteelBlue);
                row++;
            }
            ws.Cells[4, 1, ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns();
            ws.Tables.Add(ws.Cells[4, 1, ws.Dimension.End.Row, ws.Dimension.End.Column], "SummaryTable");
        }

        private static void GenerateDetailsSheet(Report report, ExcelPackage pack)
        {
            var ws = pack.Workbook.Worksheets.Add("Details");
            ws.Cells[1, 1].Value = "Type";
            ws.Cells[1, 2].Value = "Namespace";
            ws.Cells[1, 3].Value = "Class";
            ws.Cells[1, 4].Value = "Name";
            ws.Cells[1, 5].Value = "Summary ";
            ws.Cells[1, 5].AutoFitColumns();
            ws.Cells[1, 6].Value = "Parameters";
            ws.Cells[1, 6].AutoFitColumns();
            ws.Cells[1, 7].Value = "TypeParameters";
            ws.Cells[1, 7].AutoFitColumns();
            ws.Cells[1, 8].Value = "ReturnValue";
            ws.Cells[1, 8].AutoFitColumns();
            ws.Cells[1, 9].Value = "SourceFilePath";
            ws.Cells[1, 9].AutoFitColumns();
            var row = 2;
            foreach(var item in report.ReportItems.Where(r => !r.IsOK))
            {
                ws.Cells[row, 1].Value = item.ItemType;
                ws.Cells[row, 2].Value = item.Namespace;
                ws.Cells[row, 3].Value = item.Type;
                ws.Cells[row, 4].Value = item.Name;
                ws.Cells[row, 5].Value = item.Results.ContainsKey(FieldType.Summary) ? item.Results[FieldType.Summary].ToString() : "";
                ws.Cells[row, 6].Value = item.Results.ContainsKey(FieldType.Parameters) ? item.Results[FieldType.Parameters].ToString() : "";
                ws.Cells[row, 7].Value = item.Results.ContainsKey(FieldType.TypeParameters) ? item.Results[FieldType.TypeParameters].ToString() : "";
                ws.Cells[row, 8].Value = item.Results.ContainsKey(FieldType.ReturnValue) ? item.Results[FieldType.ReturnValue].ToString() : "";
                ws.Cells[row, 9].Value = item.SourceFilePath;
                row++;
            }
            ws.Cells[1, 1, Math.Min(50, ws.Dimension.End.Row), 4].AutoFitColumns();
            ws.Tables.Add(ws.Cells[1, 1, ws.Dimension.End.Row, ws.Dimension.End.Column], "Details");
        }

        private static ReportItem ValidateItem(ReflectionItem item)
        {
            ReportItem report = new ReportItem()
            {
                Uid = item.Uid,
                CommentId = item.CommentId,
                ItemType = item.ItemType.ToString(),
                Name = item.Name,
                Results = Validator.ValidateItem(item),
                SourceFilePath = item.SourceFileLocalPath
            };
            switch (item)
            {
                case ECMA2Yaml.Models.Namespace ns:
                    report.Namespace = ns.Name;
                    report.Type = "";
                    break;
                case ECMA2Yaml.Models.Type t:
                    report.Namespace = t.Parent?.Name;
                    report.Type = t.Name;
                    break;
                case ECMA2Yaml.Models.Member m:
                    report.Namespace = m.Parent?.Parent?.Name;
                    report.Type = m.Parent?.Name;
                    break;
            }

            return report;
        }
    }
}