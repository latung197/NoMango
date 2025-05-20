//==================================================================================================
// System  : DenKa
// File    : TestResultInterop.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Export CSV data to 試験結果 #101.xlsx
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using PlastMB.Common;
using PlastMB.Model;
using PlastMB.Properties;
using Microsoft.Office.Interop.Excel;
using NLog;

namespace PlastMB.Helper
{
    internal class TestResultInterop
    {
        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        // ---==== Get datas for UpdateForm ====---

        /// <summary>
        /// Get info of Excel file for Measure again
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="forces"></param>
        /// <param name="column"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static List<string> GetExcelCellsAsync(string filePath, string password, FileInfo file, CSVInfo data, ref List<string> forces, ref int column, ref int row, ref bool fillAuto, string sheetName = "データ入力")
        {
            //var datas = new string[0, 0];       // 6, 1000
            var datas = new List<string>();
            column = (int)ExportState.LotNoNotFound;

            Microsoft.Office.Interop.Excel.Application ExcelApp = null;
            Workbook Workbook = null;
            Worksheet Sheet = null;

            try
            {
                // Create a new workbook
                ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                ExcelApp.DisplayAlerts = false;
                ExcelApp.Visible = false;       //true false
                if (ExcelApp == null)
                {
                    goto L_END;
                }

                // Create a new worksheet
                if (string.IsNullOrEmpty(password))
                {
                    Workbook = ExcelApp.Workbooks.Open(filePath);
                }
                else
                {
                    //var pw = Encrypt.DecryptString(password, "think");
                    //Workbook = ExcelApp.Workbooks.Open(filePath, ReadOnly: true, Password: pw);
                    Workbook = ExcelApp.Workbooks.Open(filePath, ReadOnly: true, Password: password);
                }
                if (Workbook == null)
                {
                    goto L_END;
                }

                try
                {
                    Sheet = (Worksheet)Workbook.Worksheets.get_Item(sheetName);
                }
                catch (Exception ex)
                {
                    //Sheet = (Worksheet)Workbook.Worksheets.get_Item(1);
                    Sheet = (Worksheet)Workbook.ActiveSheet;
                    logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                }

                var ranges = (Microsoft.Office.Interop.Excel.Range)Sheet.Columns["D:D"];

                var basename = Utility.RemoveUnicode(file.Name);
                basename = Path.GetFileNameWithoutExtension(basename).ToLower();

                var row_start = 0;
                var row_end = 0;

                //if (basename.Contains(Constant.BOC_CHAM))
                if (basename.Contains(Constant.BOC_CHAM) || basename.Contains(Constant.BOC_THAP))
                {
                    // Bóc chậm -> Lực bóc tốc độ thấp
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.BOC_CHAM_ROW, ref row_start, ref row_end);
                    forces.AddRange(result);
                }
                if (basename.Contains(Constant.LUC_KEO) || basename.Contains(Constant.KEO_DUT))
                {
                    // Kéo đứt giãn EA -> Lực kéo | KEO_DUT_GIAN_EA
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.LUC_KEO_ROW, ref row_start, ref row_end);
                    forces.AddRange(result);
                }
                if (basename.Contains(Constant.DO_GIAN) && !basename.StartsWith("#2"))
                {
                    // Kéo đứt giãn HT -> Độ giãn KEO_DUT_GIAN_HT
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.DO_GIAN_ROW, ref row_start, ref row_end);
                    row = forces.Count;
                    forces.AddRange(result);
                }
                if (basename.Contains(Constant.DINH_BANG))
                {
                    // Dính bảng -> Dính bảng
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.DINH_BANG_ROW, ref row_start, ref row_end);
                    forces.AddRange(result);
                }
                //if (basename.Contains(Constant.DINH_LUNG))
                if (basename.Contains(Constant.DINH_LUNG) || basename.Contains(Constant.MAT_LUNG))
                {
                    // Dính lưng -> Dính lưng
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.DINH_LUNG_ROW, ref row_start, ref row_end);
                    forces.AddRange(result);
                }
                if (basename.Contains(Constant.XEP_CHONG))
                {
                    // Xếp chồng -> Lực xếp chồng
                    var result = GetExcelCellAsync(Sheet, ranges, Constant.XEP_CHONG_ROW, ref row_start, ref row_end);
                    forces.AddRange(result);
                }

                //var columns = new List<int>();
                //dates = GetHeaderDate(Sheet, ranges, ref columns);

                //var maxColumn = Settings.Default.MaxColumn;
                if (row_start == 0 || row_end == 0)
                {
                    goto L_END;
                }

                //datas = new string[row_end - row_start + 1, columns.Count];

                //var column = 0;
                // Updated with same LotNo, Color, NL
                column = GetMatchMeasure(Sheet, ranges, data);
                if (column <= 0)
                {
                    goto L_END;
                }

                //for (var i = 5; i < maxColumn + 5; i++)
                //foreach (var col in columns)
                for (var j = 0; j < row_end - row_start + 1; j++)
                {
                    try
                    {
                        var cell = Sheet.Cells[row_start + j, column];
                        if (cell?.Value?.GetType() == typeof(double))
                        {
                            //datas[j, column] = cell.Value.ToString();
                            //datas.Add(cell.Value.ToString());
                            //datas.Add(cell.Value.ToString("0.############"));
                            if (cell?.Value < 10)
                            {
                                var result = Math.Round(cell?.Value, 3, MidpointRounding.ToEven);
                                datas.Add(result.ToString());
                            }
                            else
                            {
                                var result = Math.Round(cell?.Value, 1, MidpointRounding.ToEven);
                                datas.Add(result.ToString());
                            }
                        }
                        else
                        {
                            //datas[j, column] = string.Empty;
                            datas.Add(string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                    }
                }
                //column++;

                var ft = Settings.Default.FillType;     // Check Auto in Line 115 when Update
                var fillType = Sheet.Cells[ft, column].Value;
                if (fillType == "Auto")
                {
                    fillAuto = true;
                }
                else
                {
                    fillAuto = false;
                }

            L_END:
                // save the workbook
                Workbook?.Save();        // Save changes to the existing file
                //Workbook?.Close();       // Close without saving changes
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The password you supplied is not correct"))
                {
                    column = (int)ExportState.PassIncorrect;
                }
                logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
            }
            finally
            {
                // After reading, relaase the excel project
                if (Sheet != null)
                {
                    try
                    {
                        ExcelApp.DisplayAlerts = false;
                        Marshal.ReleaseComObject(Sheet);
                        Sheet = null;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                    }
                }
                if (Workbook != null)
                {
                    try
                    {
                        ExcelApp.DisplayAlerts = false;
                        Workbook.Close(false, Type.Missing, Type.Missing);
                        Marshal.ReleaseComObject(Workbook);
                        Workbook = null;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                    }
                }
                if (ExcelApp != null)
                {
                    try
                    {
                        ExcelApp.Quit();
                        Marshal.ReleaseComObject(ExcelApp);
                        ExcelApp = null;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                    }
                }
            }
            return datas;
        }


        // ---==== UPDATE & EXPORT ====---

        /// <summary>
        /// Fill force result to Excel file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="datas"></param>
        /// <param name="column"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static int SetExcelCellsAsync(string filePath, string password, List<CSVInfo> datas, int column = -1, string sheetName = "データ入力")
        {
            //var isSuccess = false;
            var exportState = (int)ExportState.Error;

            Microsoft.Office.Interop.Excel.Application ExcelApp = null;
            Workbook Workbook = null;
            Worksheet Sheet = null;

            try
            {
                // Create a new workbook
                ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                ExcelApp.DisplayAlerts = false;
                ExcelApp.Visible = false;       //true
                if (ExcelApp == null)
                {
                    //return exportState;
                    goto L_END;
                }

                // Create a new worksheet
                if (string.IsNullOrEmpty(password))
                {
                    Workbook = ExcelApp.Workbooks.Open(filePath);
                }
                else
                {
                    //var pw = Encrypt.DecryptString(password, "think");
                    //Workbook = ExcelApp.Workbooks.Open(filePath, ReadOnly: false, Password: pw);
                    Workbook = ExcelApp.Workbooks.Open(filePath, ReadOnly: false, Password: password);
                }
                if (Workbook == null)
                {
                    //return exportState;
                    goto L_END;
                }

                try
                {
                    Sheet = (Worksheet)Workbook.Worksheets.get_Item(sheetName);
                }
                catch (Exception ex)
                {
                    //Sheet = (Worksheet)Workbook.Worksheets.get_Item(1);
                    Sheet = (Worksheet)Workbook.ActiveSheet;
                    logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                }

                var ranges = (Microsoft.Office.Interop.Excel.Range)Sheet.Columns["D:D"];
                var maxColumn = FindHeaderNoMax(Sheet, ranges);

                foreach (var data in datas)
                {
                    var basename = Utility.RemoveUnicode(data.File.Name);
                    basename = Path.GetFileNameWithoutExtension(basename).ToLower();

                    int times = 1;
                    bool missType = true;

                    //if (basename.Contains(Constant.BOC_CHAM))
                    if (basename.Contains(Constant.BOC_CHAM) || basename.Contains(Constant.BOC_THAP))
                    {
                        // Bóc chậm -> Lực bóc tốc độ thấp
                        SetExcelCellAsync(Sheet, ranges, Constant.BOC_CHAM_ROW, data, maxColumn, column, times);
                        //SetExcelCellAsync(Sheet, ranges, data.Program, data, maxColumn, column, times);
                        /*if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist)
                        {   // Fixbug #10082
                            //isSuccess = false;
                            //throw new Exception("Full Exception");
                            goto L_END;
                        }*/
                        times = 0;
                        missType = false;
                    }
                    if (basename.Contains(Constant.LUC_KEO) || basename.Contains(Constant.KEO_DUT))
                    {
                        // Kéo đứt giãn EA -> Lực kéo | KEO_DUT_GIAN_EA
                        SetExcelCellAsync(Sheet, ranges, Constant.LUC_KEO_ROW, data, maxColumn, column, times);
                        //SetExcelCellAsync(Sheet, ranges, data.Program, data, maxColumn, column, times);
                        //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                        times = 0;
                        missType = false;
                    }
                    if (basename.Contains(Constant.DO_GIAN) && !basename.StartsWith("#2"))
                    {
                        // Kéo đứt giãn HT -> Độ giãn KEO_DUT_GIAN_HT
                        SetExcelCellAsync(Sheet, ranges, Constant.DO_GIAN_ROW, data, maxColumn, column, times);
                        //SetExcelCellAsync(Sheet, ranges, data.Program2, data, maxColumn, column, times);
                        //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                        times = 0;
                        missType = false;
                    }
                    if (basename.Contains(Constant.DINH_BANG) && basename.Contains(Constant.DINH_LUNG))
                    {
                        // Dính bảng + Dính lưng
                        SetExcelCellAsync(Sheet, ranges, Constant.DINH_BANG_ROW, data, maxColumn, column, times, Constant.PLATE);
                        //SetExcelCellAsync(Sheet, ranges, data.Program, data, maxColumn, column, times);
                        times = 0;
                        //if (exportState != (int)ExportState.OK) goto L_END;
                        SetExcelCellAsync(Sheet, ranges, Constant.DINH_LUNG_ROW, data, maxColumn, column, times, Constant.BACKYARD);
                        //SetExcelCellAsync(Sheet, ranges, data.Program2, data, maxColumn, column, times);
                        //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                        missType = false;
                    }
                    else
                    {
                        if (basename.Contains(Constant.DINH_BANG))
                        {
                            // Dính bảng -> Dính bảng
                            SetExcelCellAsync(Sheet, ranges, Constant.DINH_BANG_ROW, data, maxColumn, column, times);
                            //SetExcelCellAsync(Sheet, ranges, data.Program, data, maxColumn, column, times);
                            //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                            times = 0;
                            missType = false;
                        }
                        //if (basename.Contains(Constant.DINH_LUNG))
                        if (basename.Contains(Constant.DINH_LUNG) || basename.Contains(Constant.MAT_LUNG))
                        {
                            // Dính lưng -> Dính lưng
                            SetExcelCellAsync(Sheet, ranges, Constant.DINH_LUNG_ROW, data, maxColumn, column, times);
                            //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                            times = 0;
                            missType = false;
                        }
                    }
                    if (basename.Contains(Constant.XEP_CHONG))
                    {
                        // Xếp chồng -> Lực xếp chồng
                        SetExcelCellAsync(Sheet, ranges, Constant.XEP_CHONG_ROW, data, maxColumn, column, times);
                        //SetExcelCellAsync(Sheet, ranges, data.Program, data, maxColumn, column, times);
                        //if (exportState != (int)ExportState.OK && exportState != (int)ExportState.Exist) goto L_END;
                        times = 0;
                        missType = false;
                    }
                    /*else
                    {
                        goto L_END;
                    }*/
                    if (missType)
                    {
                        data.InputState = ExportState.FileMissType;
                        data.ExportState = ExportState.FileMissType;
                    }
                }

                //isSuccess = true;
                exportState = (int)ExportState.OK;

            L_END:
                //Console.WriteLine(exportState);
                ;
            }
            catch (Exception ex)
            {
                foreach (var data in datas)
                {
                    data.ExportState = ExportState.PassIncorrect;
                }
                logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
            }
            finally
            {
                // save the workbook
                Workbook?.Save();        // Save changes to the existing file
                Workbook?.Close();       // Close without saving changes

                // After reading, relaase the excel project
                ExcelApp?.Quit();
                if (Workbook != null)
                {
                    Marshal.ReleaseComObject(Workbook);
                }
                if (ExcelApp != null)
                {
                    Marshal.ReleaseComObject(ExcelApp);
                }
                if (Sheet != null)
                {
                    Marshal.ReleaseComObject(Sheet);
                }
                Sheet = null;
                Workbook = null;
                ExcelApp = null;
            }
            return exportState;
        }


        // - Private Functions


        /// <summary>
        /// Get list of force by force key
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="key"></param>
        /// <param name="row_start"></param>
        /// <param name="row_end"></param>
        /// <returns></returns>
        private static List<string> GetExcelCellAsync(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, string key, ref int row_start, ref int row_end)
        {
            var datas = new List<string>();
            var row = 0;
            var col = 0;
            var key1o = $"{Constant.ROW_1}{key}";
            var key1s = $"{Constant.ROW_1} {key}";
            var key2o = $"{Constant.ROW_2}{key}";
            var key2s = $"{Constant.ROW_2} {key}";
            var key3o = $"{Constant.ROW_3}{key}";
            var key3s = $"{Constant.ROW_3} {key}";
            var key4o = $"{Constant.ROW_4}{key}";
            var key4s = $"{Constant.ROW_4} {key}";
            var key5o = $"{Constant.ROW_5}{key}";
            var key5s = $"{Constant.ROW_5} {key}";

            var range1o = ranges.Find(key1o, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range1s = ranges.Find(key1s, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range2o = ranges.Find(key2o, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range2s = ranges.Find(key2s, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range3o = ranges.Find(key3o, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range3s = ranges.Find(key3s, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range4o = ranges.Find(key4o, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range4s = ranges.Find(key4s, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range5o = ranges.Find(key5o, LookAt: XlLookAt.xlPart, MatchCase: false);
            var range5s = ranges.Find(key5s, LookAt: XlLookAt.xlPart, MatchCase: false);

            if (range1o != null)
            {
                if (row_start <= 0)
                    row_start = range1o.Row;
                //row_end = range1o.Row;
                datas.Add(Sheet.Cells[range1o.Row, range1o.Column].Value);
            }
            else if (range1s != null)
            {
                if (row_start <= 0)
                    row_start = range1s.Row;
                //row_end = range1s.Row;
                datas.Add(Sheet.Cells[range1s.Row, range1s.Column].Value);
            }

            if (range2o != null)
            {
                row = range2o.Row;
                col = range2o.Column;
                row_end = range2o.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }
            else if (range2s != null)
            {
                row = range2s.Row;
                col = range2s.Column;
                row_end = range2s.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }

            if (range3o != null)
            {
                row = range3o.Row;
                col = range3o.Column;
                row_end = range3o.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }
            else if (range3s != null)
            {
                row = range3s.Row;
                col = range3s.Column;
                row_end = range3s.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }

            if (range4o != null)
            {
                row = range4o.Row;
                col = range4o.Column;
                row_end = range4o.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }
            else if (range4s != null)
            {
                row = range4s.Row;
                col = range4s.Column;
                row_end = range4s.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }

            if (range5o != null)
            {
                row = range5o.Row;
                col = range5o.Column;
                row_end = range5o.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }
            else if (range5s != null)
            {
                row = range5s.Row;
                col = range5s.Column;
                row_end = range5s.Row;
                datas.Add(Sheet.Cells[row, col].Value);
            }

            if (row > 0)
            {
                // Kết quả đo > 1 kết quả
                // Add the Average value
                datas.Add(Sheet.Cells[row + 1, col].Value);
                row_end = row + 1;
            }
            else
            {
                // Kết quả đo chỉ có 1 kết quả - tìm (n = 1)
                if (range1o != null)
                {
                    //row_start = range1o.Row;
                    row_end = range1o.Row;
                    //datas.Add(Sheet.Cells[range1o.Row, range1o.Column].Value);
                }
                else if (range1s != null)
                {
                    //row_start = range1s.Row;
                    row_end = range1s.Row;
                    //datas.Add(Sheet.Cells[range1s.Row, range1s.Column].Value);
                }
                else
                {
                    // Kết quả đo chỉ có 1 kết quả không có (n = 1)
                    var keys = string.Empty;
                    if (key == Constant.BOC_CHAM_ROW)
                    {
                        keys = $"{Constant.BOC_CHAM_KEY}";
                    }
                    else
                    {
                        keys = $"{key}";
                    }
                    var range = ranges.Find(keys, LookAt: XlLookAt.xlPart, MatchCase: false);
                    if (range != null)
                    {
                        row_start = range.Row;
                        row_end = range.Row;
                        datas.Add(Sheet.Cells[range.Row, range.Column].Value);
                    }
                }
            }
            //FillHeaderNo(Sheet, ranges);
            return datas;
        }

        /// <summary>
        /// Fill force result by force key
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="column"></param>
        private static int SetExcelCellAsync(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, string key, CSVInfo data, int maxColumn, int column = -1, int times = 1, string key2 = "")
        {
            try
            {
                var row = 0;
                var col = 0;
                var key1o = $"{Constant.ROW_1}{key}";
                var key1s = $"{Constant.ROW_1} {key}";
                var key2o = $"{Constant.ROW_2}{key}";
                var key2s = $"{Constant.ROW_2} {key}";
                var key3o = $"{Constant.ROW_3}{key}";
                var key3s = $"{Constant.ROW_3} {key}";
                var key4o = $"{Constant.ROW_4}{key}";
                var key4s = $"{Constant.ROW_4} {key}";
                var key5o = $"{Constant.ROW_5}{key}";
                var key5s = $"{Constant.ROW_5} {key}";

                var range1o = ranges.Find(key1o, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range1s = ranges.Find(key1s, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range2o = ranges.Find(key2o, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range2s = ranges.Find(key2s, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range3o = ranges.Find(key3o, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range3s = ranges.Find(key3s, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range4o = ranges.Find(key4o, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range4s = ranges.Find(key4s, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range5o = ranges.Find(key5o, LookAt: XlLookAt.xlPart, MatchCase: false);
                var range5s = ranges.Find(key5s, LookAt: XlLookAt.xlPart, MatchCase: false);

                //var maxColumn = FindHeaderNoMax(Sheet, ranges);
                var oldcolumn = 0;

                if (range2o != null)
                {
                    row = range2o.Row;
                    col = range2o.Column;
                    if (column >= 0)
                    {
                        // Fill old cell with same LotNo, Color, NL
                        var val = data.GetForceResult(1, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key2o}: {val}");
                    }
                    else
                    {
                        // Tìm các thông số LotNo, Color, NL đã được fill ở header chưa !?

                        oldcolumn = GetMatchMeasure(Sheet, ranges, data, maxColumn);
                        if (oldcolumn > 0)
                        {   // Đã tồn tại cột có cùng LotNo
                            var value = Sheet.Cells[range2o.Row, oldcolumn].Value;
                            if (value != null)
                            {   // Cell đã có giá trị -> Báo lỗi: Các thông số LotNo đã có trong tệp Excel
                                //Constant.FILE_ERROR = data.File.Name;
                                //Constant.File_Exist.Add(data);
                                data.ExportState = ExportState.Exist;
                                return (int)ExportState.Exist;
                            }
                            else
                            {
                                var ft = Settings.Default.FillType;     // Check Auto in Line 115 when Export
                                var fillType = Sheet.Cells[ft, oldcolumn].Value;
                                if (fillType == "Auto")
                                {   // If is Auto => Export
                                    // Fill cell with same LotNo, Color, NL
                                    var val = data.GetForceResult(1, key, key2);
                                    FillCell(Sheet, row, oldcolumn, val);
                                    logger.Debug($"Export to row {row}, old column {oldcolumn} {key2o} ({fillType}): {val}");
                                }
                                else
                                {   // Else => Warning
                                    data.ExportState = ExportState.ExportCraft;
                                    data.column = oldcolumn;
                                    return (int)ExportState.ExportCraft;
                                }
                            }
                        }
                        else
                        {
                            // Find header cell with empty LotNo, Color, NL
                            oldcolumn = FindHeaderLotEmpty(Sheet, ranges, data, maxColumn);
                            if (oldcolumn > 0)
                            {
                                var val = data.GetForceResult(1, key, key2);
                                FillCell(Sheet, row, oldcolumn, val);
                                logger.Debug($"Export to row {row}, column {oldcolumn} {key2o}: {val}");
                            }
                            else
                            {   // TODO remove it
                                // Find cell empty in row
                                //var cend = FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(1, key, key2));
                                //var cend = FillCellEmpty(Sheet, maxColumn, row, range2o.Column, data.GetForceResult(1, key, key2));
                                //if (cend == range2o.Column) {
                                data.ExportState = ExportState.Full;
                                return (int)ExportState.Full;
                            }
                        }
                    }
                }
                else if (range2s != null)
                {
                    row = range2s.Row;
                    col = range2s.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(1, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key2s}: {val}");
                    }
                    else
                    {
                        // Fill cell with same LotNo, Color, NL
                        oldcolumn = GetMatchMeasure(Sheet, ranges, data, maxColumn);
                        if (oldcolumn > 0)
                        {
                            var value = Sheet.Cells[range2s.Row, oldcolumn].Value;
                            if (value != null)
                            {
                                data.ExportState = ExportState.Exist;
                                return (int)ExportState.Exist;
                            }
                            else
                            {
                                var ft = Settings.Default.FillType;     // Check Auto in Line 115 when Export
                                var fillType = Sheet.Cells[ft, oldcolumn].Value;
                                if (fillType == "Auto")
                                {   // If is Auto => Export
                                    var val = data.GetForceResult(1, key, key2);
                                    FillCell(Sheet, row, oldcolumn, val);
                                    logger.Debug($"Export to row {row}, old column {oldcolumn} {key2s} ({fillType}): {val}");
                                }
                                else
                                {   // Else => Warning
                                    data.ExportState = ExportState.ExportCraft;
                                    data.column = oldcolumn;
                                    return (int)ExportState.ExportCraft;
                                }
                            }
                        }
                        else
                        {
                            // Find header cell with empty LotNo, Color, NL
                            oldcolumn = FindHeaderLotEmpty(Sheet, ranges, data, maxColumn);
                            if (oldcolumn > 0)
                            {
                                var val = data.GetForceResult(1, key, key2);
                                FillCell(Sheet, row, oldcolumn, val);
                                logger.Debug($"Export to row {row}, column {oldcolumn} {key2s}: {val}");
                            }
                            else
                            {
                                // TODO remove it
                                //var cend = FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(1, key, key2));
                                //var cend = FillCellEmpty(Sheet, maxColumn, row, range2s.Column, data.GetForceResult(1, key, key2));
                                //if (cend == range2s.Column) {
                                data.ExportState = ExportState.Full;
                                return (int)ExportState.Full;
                            }
                        }
                    }
                }

                if (range3o != null)
                {
                    row = range3o.Row;
                    col = range3o.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(2, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key3o}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(2, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key3o}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(2, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }
                else if (range3s != null)
                {
                    row = range3s.Row;
                    col = range3s.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(2, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key3s}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(2, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key3s}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(2, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }

                if (range4o != null)
                {
                    row = range4o.Row;
                    col = range4o.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(3, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key4o}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(3, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key4o}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(3, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }
                else if (range4s != null)
                {
                    row = range4s.Row;
                    col = range4s.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(3, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key4s}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(3, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key4s}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(3, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }

                if (range5o != null)
                {
                    row = range5o.Row;
                    col = range5o.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(4, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key5o}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(4, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key5o}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(4, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }
                else if (range5s != null)
                {
                    row = range5s.Row;
                    col = range5s.Column;
                    if (column >= 0)
                    {
                        var val = data.GetForceResult(4, key, key2);
                        FillCell(Sheet, row, column, val);
                        logger.Debug($"Overwrite to row {row}, column {column} {key5s}: {val}");
                    }
                    else
                    {
                        if (oldcolumn > 0)
                        {
                            var val = data.GetForceResult(4, key, key2);
                            FillCell(Sheet, row, oldcolumn, val);
                            logger.Debug($"Export to row {row}, column {oldcolumn} {key5s}: {val}");
                        }
                        else
                        {
                            // TODO remove it
                            //FillCellEmpty(Sheet, maxColumn, row, col, data.GetForceResult(4, key, key2));
                            data.ExportState = ExportState.Full;
                            return (int)ExportState.Full;
                        }
                    }
                }

                if (row > 0)
                {
                    // CASE1:: Kết quả đo > 1 kết quả
                    var rstart = 0;
                    var cend = 0;
                    if (range1o != null)
                    {
                        rstart = range1o.Row;
                        if (column >= 0)
                        {
                            var val = data.GetForceResult(0, key, key2);
                            FillCell(Sheet, rstart, column, val);
                            logger.Debug($"Overwrite to row {rstart}, column {column} {key1o}: {val}");
                        }
                        else
                        {
                            if (oldcolumn > 0)
                            {
                                var val = data.GetForceResult(0, key, key2);
                                FillCell(Sheet, rstart, oldcolumn, val);
                                logger.Debug($"Export to row {rstart}, column {oldcolumn} {key1o}: {val}");
                                //cend = oldcolumn;
                            }
                            else
                            {
                                //cend = FillCellEmpty(Sheet, maxColumn, rstart, range1o.Column, data.GetForceResult(0, key, key2));
                                //if (cend == range1o.Column)
                                data.ExportState = ExportState.Full;
                                return (int)ExportState.Full;
                            }
                        }
                    }
                    else if (range1s != null)
                    {
                        rstart = range1s.Row;
                        if (column >= 0)
                        {
                            var val = data.GetForceResult(0, key, key2);
                            FillCell(Sheet, rstart, column, val);
                            logger.Debug($"Overwrite to row {rstart}, column {column} {key1s}: {val}");
                        }
                        else
                        {
                            if (oldcolumn > 0)
                            {
                                var val = data.GetForceResult(0, key, key2);
                                FillCell(Sheet, rstart, oldcolumn, val);
                                logger.Debug($"Export to row {rstart}, column {oldcolumn} {key1s}: {val}");
                                //cend = oldcolumn;
                            }
                            else
                            {
                                //cend = FillCellEmpty(Sheet, maxColumn, rstart, range1s.Column, data.GetForceResult(0, key, key2));
                                //if (cend == range1s.Column)
                                data.ExportState = ExportState.Full;
                                return (int)ExportState.Full;
                            }
                        }
                    }

                    // Khi fill giá trị chỉ được paste giá trị, không được paste format (không được làm thay đổi format hoặc công thức tính)
                    /*if (column >= 0)
                    {
                        var columnName = GetExcelColumnName(column);
                        var formula = $"=IF({columnName}{rstart}<>\"\",AVERAGE({columnName}{rstart}:{columnName}{row}),\"\")";
                        Sheet.Cells[row + 1, column].Formula = formula;
                    }
                    else
                    {
                        var columnName = GetExcelColumnName(cend);
                        //var formula = "=IF(E30<>\"\",AVERAGE(E30:E34),\"\")";
                        //var formula = $"=IF(E{rstart}<>\"\",AVERAGE(E{rstart}:E{row}),\"\")";
                        var formula = $"=IF({columnName}{rstart}<>\"\",AVERAGE({columnName}{rstart}:{columnName}{row}),\"\")";
                        Sheet.Cells[row + 1, cend].Formula = formula;
                    }*/

                    if (column >= 0)
                    {
                        if (times == 1)
                        {
                            // Fill Line #114: Number of measurements
                            FillHeaderMeasure(Sheet, column);
                        }
                    }
                    else if (oldcolumn <= 0)
                    {
                        // Fill Line #2: Length, #4: 品種, #5: 品種, #9: 包装ロットLô đóng gói, Line #12: Color, Line 114: Number of measurements, Line 115: Auto
                        SetHeaderMeasure(Sheet, ranges, data, cend);
                    }
                }
                else
                {
                    // CASE2:: Kết quả đo chỉ có 1 kết quả - tìm (n = 1)KEY
                    var cend = 0;
                    if (range1o != null)
                    {
                        if (column >= 0)
                        {
                            // Fill old cell with same LotNo, Color, NL
                            var val = data.GetForceAverage(key2);
                            FillCell(Sheet, range1o.Row, column, val);
                            logger.Debug($"Overwrite to row {range1o.Row}, column {column} {key1o}: {val}");
                        }
                        else
                        {
                            // Tìm các thông số LotNo, Color, NL đã được fill ở header chưa !?
                            // Fill cell with same LotNo, Color, NL
                            oldcolumn = GetMatchMeasure(Sheet, ranges, data, maxColumn);
                            if (oldcolumn > 0)
                            {
                                var value = Sheet.Cells[range1o.Row, oldcolumn].Value;
                                if (value != null)
                                {
                                    data.ExportState = ExportState.Exist;
                                    return (int)ExportState.Exist;
                                }
                                else
                                {
                                    // Check Auto in Line 115
                                    var ft = Settings.Default.FillType;
                                    var fillType = Sheet.Cells[ft, oldcolumn].Value;
                                    if (fillType == "Auto")
                                    {   // If is Auto => Export
                                        var val = data.GetForceAverage(key2);
                                        FillCell(Sheet, range1o.Row, oldcolumn, val);
                                        logger.Debug($"Export to row {range1o.Row}, old column {oldcolumn} {key1o} ({fillType}): {val}");
                                    }
                                    else
                                    {   // Else => Warning
                                        data.ExportState = ExportState.ExportCraft;
                                        data.column = oldcolumn;
                                        return (int)ExportState.ExportCraft;
                                    }
                                }
                            }
                            else
                            {
                                // Find header cell with empty LotNo, Color, NL
                                oldcolumn = FindHeaderLotEmpty(Sheet, ranges, data, maxColumn);
                                if (oldcolumn > 0)
                                {
                                    var val = data.GetForceAverage(key2);
                                    FillCell(Sheet, range1o.Row, oldcolumn, val);
                                    logger.Debug($"Export to row {range1o.Row}, column {oldcolumn} {key1o}: {val}");
                                }
                                else
                                {
                                    // Find cell empty in row
                                    //cend = FillCellEmpty(Sheet, maxColumn, range1o.Row, range1o.Column, data.GetForceAverage(key2));
                                    //if (cend == range1o.Column)
                                    data.ExportState = ExportState.Full;
                                    return (int)ExportState.Full;
                                }
                            }
                        }
                    }
                    else if (range1s != null)
                    {
                        //  CASE2b:: Kết quả đo chỉ có 1 kết quả - tìm (n = 1) KEY
                        if (column >= 0)
                        {
                            var val = data.GetForceAverage(key2);
                            FillCell(Sheet, range1s.Row, column, val);
                            logger.Debug($"Overwrite to row {range1s.Row}, column {column} {key1s}: {val}");
                        }
                        else
                        {
                            // Fill cell with same LotNo, Color, NL
                            oldcolumn = GetMatchMeasure(Sheet, ranges, data, maxColumn);
                            if (oldcolumn > 0)
                            {
                                var value = Sheet.Cells[range1s.Row, oldcolumn].Value;
                                if (value != null)
                                {
                                    data.ExportState = ExportState.Exist;
                                    return (int)ExportState.Exist;
                                }
                                else
                                {
                                    // Check Auto in Line 115
                                    var ft = Settings.Default.FillType;
                                    var fillType = Sheet.Cells[ft, oldcolumn].Value;
                                    if (fillType == "Auto")
                                    {   // If is Auto => Export
                                        var val = data.GetForceAverage(key2);
                                        FillCell(Sheet, range1s.Row, oldcolumn, val);
                                        logger.Debug($"Export to row {range1s.Row}, old column {oldcolumn} {key1s} ({fillType}): {val}");
                                    }
                                    else
                                    {   // Else => Warning
                                        data.ExportState = ExportState.ExportCraft;
                                        data.column = oldcolumn;
                                        return (int)ExportState.ExportCraft;
                                    }
                                }
                            }
                            else
                            {
                                // Find header cell with empty LotNo, Color, NL
                                oldcolumn = FindHeaderLotEmpty(Sheet, ranges, data, maxColumn);
                                if (oldcolumn > 0)
                                {
                                    var val = data.GetForceAverage(key2);
                                    FillCell(Sheet, range1s.Row, oldcolumn, val);
                                    logger.Debug($"Export to row {range1s.Row}, column {oldcolumn} {key1s}: {val}");
                                }
                                else
                                {
                                    // Find cell empty in row
                                    //cend = FillCellEmpty(Sheet, maxColumn, range1s.Row, range1s.Column, data.GetForceAverage(key2));
                                    //if (cend == range1s.Column)
                                    data.ExportState = ExportState.Full;
                                    return (int)ExportState.Full;
                                }
                            }
                        }
                    }
                    else
                    {
                        // CASE3:: Kết quả đo chỉ có 1 kết quả và Không có (n = 1)
                        var keys = string.Empty;
                        if (key == Constant.BOC_CHAM_ROW)
                        {
                            keys = $"{Constant.BOC_CHAM_KEY}";
                        }
                        else
                        {
                            keys = $"{key}";
                        }
                        var range = ranges.Find(keys, LookAt: XlLookAt.xlPart, MatchCase: false);
                        if (range != null)
                        {
                            if (column >= 0)
                            {
                                // Fill old cell with same LotNo, Color, NL
                                var val = data.GetForceAverage(key2);
                                FillCell(Sheet, range.Row, column, val);
                                logger.Debug($"Overwrite to row {range.Row}, column {column} ({keys}): {val}");
                            }
                            else
                            {
                                // Tìm các thông số LotNo, Color, NL đã được fill ở header chưa !?
                                // Fill cell with same LotNo, Color, NL
                                oldcolumn = GetMatchMeasure(Sheet, ranges, data, maxColumn);
                                if (oldcolumn > 0)
                                {
                                    var value = Sheet.Cells[range.Row, oldcolumn].Value;
                                    if (value != null)
                                    {
                                        data.ExportState = ExportState.Exist;
                                        return (int)ExportState.Exist;
                                    }
                                    else
                                    {
                                        var ft = Settings.Default.FillType;     // Check Auto in Line 115 when Export
                                        var fillType = Sheet.Cells[ft, oldcolumn].Value;
                                        if (fillType == "Auto")
                                        {   // If is Auto => Export
                                            var val = data.GetForceAverage(key2);
                                            FillCell(Sheet, range.Row, oldcolumn, val);
                                            logger.Debug($"Export to row {range.Row}, old column {oldcolumn} {keys} ({fillType}): {val}");
                                        }
                                        else
                                        {   // Else => Warning
                                            data.ExportState = ExportState.ExportCraft;
                                            data.column = oldcolumn;
                                            return (int)ExportState.ExportCraft;
                                        }
                                    }
                                }
                                else
                                {
                                    // Find header cell with empty LotNo, Color, NL
                                    oldcolumn = FindHeaderLotEmpty(Sheet, ranges, data, maxColumn);
                                    if (oldcolumn > 0)
                                    {
                                        var val = data.GetForceAverage(key2);
                                        FillCell(Sheet, range.Row, oldcolumn, val);
                                        logger.Debug($"Export to row {range.Row}, column {oldcolumn} {keys}: {val}");
                                    }
                                    else
                                    {
                                        // Find cell empty in row
                                        //cend = FillCellEmpty(Sheet, maxColumn, range.Row, range.Column, data.GetForceAverage(key2));
                                        //if (cend == range.Column)
                                        data.ExportState = ExportState.Full;
                                        return (int)ExportState.Full;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Constant.FILE_ERROR = data.File.Name;
                            data.ExportState = ExportState.NotFound;
                            return (int)ExportState.NotFound;
                        }
                    }

                    if (column >= 0)
                    {
                        //if (key2 != Constant.BACKYARD)
                        if (times == 1)
                        {
                            // Fill Line #114: Number of measurements
                            FillHeaderMeasure(Sheet, column);
                        }
                    }
                    else if (oldcolumn <= 0)
                    {
                        // Fill Line #2: Length, #4: 品種, #5: 品種, #9: 包装ロットLô đóng gói, Line #12: Color, Line 114: Number of measurements, Line 115: Auto
                        SetHeaderMeasure(Sheet, ranges, data, cend);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                return (int)ExportState.Error;
            }

            //FillHeaderNo(Sheet, ranges);
            return (int)ExportState.OK;
        }

        /// <summary>
        /// Convert a column number into an Excel column
        /// </summary>
        /// <param name="columnNumber">e.g. 127</param>
        /// <returns>Excel column, e.g. AA</returns>
        /*private static string GetExcelColumnName(int columnNumber)
        {
            var columnName = "";
            while (columnNumber > 0)
            {
                var modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }*/

        /// <summary>
        /// Fill cell empty in row
        /// </summary>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        /*private static int FillCellEmpty(Worksheet Sheet, int max, int row, int col, double? data)
        {
            //var ranges = Sheet.Range[Sheet.Cells[row, col + 1], Sheet.Cells[row, col + 1000]];
            var ranges = Sheet.Range[Sheet.Cells[row, col], Sheet.Cells[row, max]];
            var range = ranges.Find("", LookAt: XlLookAt.xlWhole, MatchCase: false);
            if (range != null)
            {
                if (data != null && data != 0.0f)
                {
                    Sheet.Cells[range.Row, range.Column].Value = data;
                }
                else
                {
                    Sheet.Cells[range.Row, range.Column].Value = "";
                }
                return range.Column;
            }
            return col;
        }*/

        /// <summary>
        /// Fill cell with row and column
        /// </summary>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        private static void FillCell(Worksheet Sheet, int row, int col, double? data)
        {
            if (data != null && data != 0.0f)
            {
                Sheet.Cells[row, col].Value = data;
            }
            else
            {
                Sheet.Cells[row, col].Value = "";
            }
        }

        /// <summary>
        /// Fill Number of measurements to Line #114
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        private static void FillHeaderMeasure(Worksheet Sheet, int column, int row = 0)
        {
            if (column < 0) return;

            // Fill Line 114: Number of measurements
            if (row <= 0)
            {
                row = Settings.Default.Measure;
            }
            var value = Sheet.Cells[row, column].Value;
            if (value != null)
            {
                if (value?.GetType() == typeof(int) || value?.GetType() == typeof(double))
                {
                    Sheet.Cells[row, column].Value = value + 1;
                }
                //if (value?.GetType() == typeof(string))
                else
                {
                    try
                    {
                        int result = int.Parse(value);
                        Sheet.Cells[row, column].Value = result + 1;
                        //Console.WriteLine(result);
                    }
                    catch (FormatException)
                    {
                        Sheet.Cells[row, column].Value = value + " (1)";
                    }
                }
            }
            else
            {
                Sheet.Cells[row, column].Value = 1;     // "1"
            }

            // Fill Line 115: Auto
            var rowtype = Settings.Default.FillType;
            Sheet.Cells[rowtype, column].Value = "Auto";
        }


        /// <summary>
        /// Fill NL to Line #4
        /// Fill LotNo to Line #5
        /// Fill 包装ロットLô đóng gói to Line #9
        /// Fill Color to Line #12
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="column"></param>
        /// <param name="data"></param>
        /// <param name="header"></param>
        /// <param name="rowoffset"></param>
        private static void FillHeader(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, int column, string data, string header, int rowoffset = 0)
        {
            if (column < 0) return;
            var headerNo = ranges.Find(header, LookAt: XlLookAt.xlPart, MatchCase: false);
            if (headerNo != null)
            {
                Sheet.Cells[headerNo.Row - rowoffset, column].Value = data;
            }
            else if (header == Constant.VARIETY)
            {
                var row = Settings.Default.Variety;
                Sheet.Cells[row - rowoffset, column].Value = data;
            }
        }

        /// <summary>
        /// Find cell with same LotNo, Color, NL
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int GetMatchMeasure(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, CSVInfo data, int maxColumn = 0)
        {
            var column = -1;
            if (maxColumn <= 0)
            {
                //var maxColumn = Settings.Default.MaxColumn;
                maxColumn = FindHeaderNoMax(Sheet, ranges);
            }

            // Find Line #12: Color
            try
            {
                var headerColor = ranges.Find(Constant.COLOR, LookAt: XlLookAt.xlPart, MatchCase: false);
                if (headerColor != null)
                {
                    for (var i = headerColor.Column + 1; i <= maxColumn + headerColor.Column; i++)
                    {
                        // Updated with same Color
                        var value = Sheet.Cells[headerColor.Row, i].Value;
                        if (value == null) continue;
                        if (value.ToString() != data.Color) continue;

                        // Updated with same LotNo
                        // Find Line #9: 包装ロットLô đóng gói
                        // Find Line #5: 品種
                        var headerLotno = ranges.Find(Constant.LOTNO, LookAt: XlLookAt.xlPart, MatchCase: false);
                        if (headerLotno == null) continue;
                        var lotno = Sheet.Cells[headerLotno.Row, i].Value;
                        if (lotno.ToString() != data.Lot) continue;

                        // Updated with same NL
                        /*var headerNL = ranges.Find(Constant.VARIETY, LookAt: XlLookAt.xlPart, MatchCase: false);
                        int row;
                        if (headerNL == null)
                        {
                            row = Settings.Default.Variety;
                            //continue;
                        }
                        else
                        {
                            row = headerNL.Row;
                        }
                        var nl = Sheet.Cells[row - 1, i].Value;*/
                        var row = Settings.Default.NL;
                        var nl = Sheet.Cells[row, i].Value;
                        //if (nl == null && data.NL) continue;
                        //if (nl != null && !data.NL) continue;
                        if (data.NL)
                        {
                            if (nl == null) continue;
                            if (nl.ToString() != "NL") continue;
                        }
                        else
                        {
                            if (nl != null) continue;
                            //if (nl?.ToString() != "") continue;
                        }

                        // Updated with same Production Line
                        var headerLotnoLine = ranges.Find("Ｎｏ．", LookAt: XlLookAt.xlPart, MatchCase: false);
                        if (headerLotnoLine == null) continue;
                        var lotnoLine = Sheet.Cells[headerLotnoLine.Row - 1, i].Value;
                        //if (string.IsNullOrEmpty(lotnoLine)) continue;
                        if (lotnoLine == null)
                        {
                            lotnoLine = lotno;
                        }
                        else if (lotnoLine.GetType() != typeof(string))
                        {
                            continue;
                        }
                        else if (string.IsNullOrEmpty(lotnoLine))
                        {
                            lotnoLine = lotno;
                        }
                        if (lotnoLine != data.LotNo) continue;

                        // NoEmptySkip
                        var noEmptySkip = Settings.Default.NoEmptySkip;
                        if (noEmptySkip)
                        {
                            var noValue = Sheet.Cells[headerLotnoLine.Row, i].Value;
                            if (noValue == null) continue;
                        }

                        //Console.WriteLine(string.Format("{0} {1} {2} {3}", i, lotno, value, nl));
                        column = i;
                        return column;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "TestResultInterop.cs"));
                return column;
            }
            return column;
        }

        /// <summary>
        /// Find empty column to fill LotNo, Color, NL
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int FindHeaderLotEmpty(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, CSVInfo data, int maxColumn = 0)
        {
            var column = -1;
            if (maxColumn <= 0)
            {
                maxColumn = FindHeaderNoMax(Sheet, ranges);
            }

            // Find Line #9: 包装ロットLô đóng gói
            var headerLotno = ranges.Find(Constant.LOTNO, LookAt: XlLookAt.xlPart, MatchCase: false);
            if (headerLotno != null)
            {
                // Find empty cell in Line #9: 包装ロットLô đóng gói
                //var lotRanges = Sheet.Range[Sheet.Cells[headerLotno.Row, headerLotno.Column + 1], Sheet.Cells[headerLotno.Row, maxColumn]];
                var lotRanges = Sheet.Range[Sheet.Cells[headerLotno.Row, headerLotno.Column], Sheet.Cells[headerLotno.Row, maxColumn]];
                //var range = lotRanges.Find("", LookAt: XlLookAt.xlWhole, MatchCase: false);
                /*if (range != null)
                {
                    if (range.Column == maxColumn)
                    {
                        var headerNo = ranges.Find("Ｎｏ．", LookAt: XlLookAt.xlPart, MatchCase: false);
                        if (headerNo != null)
                        {
                            var no = Sheet.Cells[headerNo.Row, range.Column].Value;
                            if (no == null)
                            {
                                return column;
                            }
                        }
                    }
                    // Fill Line #2: Length, #4: 品種, #5: 品種, #9: 包装ロットLô đóng gói, Line #12: Color, Line 114: Number of measurements, Line 115: Auto
                    SetHeaderMeasure(Sheet, ranges, data, range.Column);
                    return range.Column;
                }*/

                var headerNo = ranges.Find("Ｎｏ．", LookAt: XlLookAt.xlPart, MatchCase: false);
                if (headerNo == null)
                {
                    return column;
                }
                Microsoft.Office.Interop.Excel.Range? rangeFirstFind = null;
                Microsoft.Office.Interop.Excel.Range? rangeCurrentFind = lotRanges.Find("", LookAt: XlLookAt.xlWhole, MatchCase: false);
                while (rangeCurrentFind != null)
                {
                    if (rangeFirstFind == null)
                    {
                        rangeFirstFind = rangeCurrentFind;
                    }
                    else if (rangeCurrentFind.get_Address(XlReferenceStyle.xlA1) == rangeFirstFind.get_Address(XlReferenceStyle.xlA1))
                    {
                        break;
                    }
                    var no = Sheet.Cells[headerNo.Row, rangeCurrentFind.Column].Value;
                    if (no != null)
                    {
                        // Fill Line #2: Length, #4: 品種, #5: 品種, #9: 包装ロットLô đóng gói, Line #12: Color, Line 114: Number of measurements, Line 115: Auto
                        SetHeaderMeasure(Sheet, ranges, data, rangeCurrentFind.Column);
                        return rangeCurrentFind.Column;
                    }
                    rangeCurrentFind = lotRanges.FindNext(rangeCurrentFind);
                }
                rangeFirstFind = null;
                rangeCurrentFind = null;
            }
            return column;
        }

        /// <summary>
        /// Fill LotNo, Color, NL into empty header
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="data"></param>
        private static void SetHeaderMeasure(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, CSVInfo data, int column)
        {
            if (data.NL)
            {
                // Fill Line #4: 品種
                //FillHeader(Sheet, ranges, column, "NL", Constant.VARIETY, 1);
                var row = Settings.Default.NL;
                Sheet.Cells[row, column].Value = "NL";
            }

            // Fill Line #5: 品種
            FillHeader(Sheet, ranges, column, data.LotNo, Constant.VARIETY);

            // Fill Line #9: 包装ロットLô đóng gói
            var match = Regex.Match(data.Lot, @"\b\d+E\d+\b");
            if (match.Success)
            {
                FillHeader(Sheet, ranges, column, "'" + data.Lot, Constant.LOTNO);
            }
            else
            {
                FillHeader(Sheet, ranges, column, data.Lot, Constant.LOTNO);
            }

            // Fill Line #12: Color 色 Màu
            FillHeader(Sheet, ranges, column, data.Color, Constant.COLOR);

            // Fill Line #114: Number of measurements
            FillHeaderMeasure(Sheet, column);

            // Fill Line 115: Auto
            var rowtype = Settings.Default.FillType;
            Sheet.Cells[rowtype, column].Value = "Auto";

            // Fill Line 2: Length
            var rowlength = Settings.Default.LengthFill;
            Sheet.Cells[rowlength, column].Value = data.Length;
        }

        /// <summary>
        /// Find max of No header
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private static int FindHeaderNoMax(Worksheet Sheet, Microsoft.Office.Interop.Excel.Range ranges, string header = "Ｎｏ．")
        {
            var max = 0;

            var usedRange = Sheet.UsedRange;
            max = usedRange.Columns.Count;
            if (max > 0)
            {
                return max;
            }

            var headerNo = ranges.Find(header, LookAt: XlLookAt.xlPart, MatchCase: false);
            if (headerNo != null)
            {
                var i = 5;
                while (true)
                {
                    var value = Sheet.Cells[headerNo.Row, i].Value;
                    if (value == null)
                    {
                        //break;
                        return i - 1;
                    }
                    i++;
                }
            }
            return max;
        }

        /// <summary>
        /// Fill Ｎｏ． to Line #6
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="header"></param>
        /*private static void FillHeaderNo(Worksheet Sheet, Range ranges, string header = "Ｎｏ．")
        {
            var headerNo = ranges.Find(header, LookAt: XlLookAt.xlPart, MatchCase: false);
            if (headerNo != null)
            {
                var maxColumn = Settings.Default.MaxColumn;     // 2000
                var _ranges = Sheet.Range[Sheet.Cells[headerNo.Row, headerNo.Column + 1], Sheet.Cells[headerNo.Row, headerNo.Column + maxColumn]];
                var _range = _ranges.Find("", LookAt: XlLookAt.xlWhole, MatchCase: false);
                if (_range != null)
                {
                    var lastNo = Sheet.Cells[_range.Row, _range.Column - 1].Value;
                    if (lastNo.GetType() == typeof(double))
                    {
                        Sheet.Cells[_range.Row, _range.Column].Value = lastNo + 1;
                    }
                }
            }
        }*/

        /// <summary>
        /// Get list of date time has measured
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="ranges"></param>
        /// <param name="columns"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        /*private static List<DateTime> GetHeaderDate(Worksheet Sheet, Range ranges, ref List<int> columns, string header = "検査日Ngày kiểm tra")
        {
            var dates = new List<DateTime>();
            var headerNo = ranges.Find(header, LookAt: XlLookAt.xlPart, MatchCase: false);
            if (headerNo != null)
            {
                var maxColumn = Settings.Default.MaxColumn;
                var dateFormat = Settings.Default.DateFormat;
                for (var i = headerNo.Column + 1; i < maxColumn; i++)
                {
                    try
                    {
                        var cell = Sheet.Cells[headerNo.Row, i];
                        if (cell != null)
                        {
                            //var value = Sheet.Cells[headerNo.Row, i].Value;
                            if (cell.Value?.GetType() == typeof(string))
                            {
                                var date = Utility.GetDate(cell.Value, dateFormat);
                                if (date != null)
                                {
                                    dates.Add(date);
                                    columns.Add(i);
                                }
                                //Console.WriteLine(value.ToString());
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
            return dates;
        }*/
    }
}