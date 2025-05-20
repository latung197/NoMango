//==================================================================================================
// System  : DenKa
// File    : TestResult.cs
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
using System.Text.RegularExpressions;
using PlastMB.Common;
using PlastMB.Model;
using PlastMB.Properties;

namespace PlastMB.Helper
{

    internal class TestResultHelper
    {
        // ---==== UPDATE ====---

        public static int SetExcelCellsAsync(string folderPath, List<CSVInfo> datas, int column = -1, string sheetName = "データ入力")
        {
            var exportState = (int)ExportState.Error;

            var data101Mikasa = new List<CSVInfo>();
            var data101Sekisui = new List<CSVInfo>();
            var data101Yazaki = new List<CSVInfo>();
            var data101 = new List<CSVInfo>();
            var data102 = new List<CSVInfo>();
            var data103 = new List<CSVInfo>();
            var data106U = new List<CSVInfo>();
            var data134 = new List<CSVInfo>();
            var data230W = new List<CSVInfo>();
            var data232W = new List<CSVInfo>();
            var data234W = new List<CSVInfo>();
            var data246W = new List<CSVInfo>();
            var data248WS = new List<CSVInfo>();
            var data258WS = new List<CSVInfo>();
            var data260W = new List<CSVInfo>();
            var data261L = new List<CSVInfo>();

            //try{
            foreach (var data in datas)
            {
                var basename = Path.GetFileNameWithoutExtension(data.File.Name);
                basename = basename.Replace(" ", "");
                basename = basename.Replace("(", "");
                basename = basename.Replace(")", "");
                basename = basename.Replace("（", "");
                basename = basename.Replace("）", "");
                basename = basename.ToUpper();

                //if (basename.Contains(Constant.TEST_RESULT_101_MIKASA))
                if (basename.Contains(Constant.TEST_RESULT_101_MIKASA_SHORT))
                {   // 試験結果 #101 (Mikasa)
                    data101Mikasa.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_101_SEKISUI_SHORT))
                {   // 試験結果 #101 (Sekisui)  TEST_RESULT_101_SEKISUI
                    data101Sekisui.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_101_YAZAKI_SHORT))
                {   // 試験結果 #101 (Yazaki)   TEST_RESULT_101_YAZAKI
                    data101Yazaki.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_101))
                {   // 試験結果 #101
                    data101.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_102))
                {   // 試験結果 #102
                    data102.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_103))
                {   // 試験結果 #103
                    data103.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_106U))
                {   // 試験結果 #106U
                    data106U.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_134))
                {   // 試験結果 #134
                    data134.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_230W))
                {   // 試験結果 #230W
                    data230W.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_232W))
                {   // 試験結果 #232W
                    data232W.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_234W))
                {   // 試験結果 #234W
                    data234W.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_246W))
                {   // 試験結果 #246W
                    data246W.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_248WS))
                {   // 試験結果 #248WS
                    data248WS.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_258WS))
                {   // 試験結果 #258WS
                    data258WS.Add(data);
                }
                //else if (basename.Contains(Constant.TEST_RESULT_260W) || basename.Contains(Constant.TEST_RESULT_260W_SHORT))
                else if (basename.Contains(Constant.TEST_RESULT_260W))
                {   // 試験結果 #260W
                    data260W.Add(data);
                }
                else if (basename.Contains(Constant.TEST_RESULT_261L))
                {   // 試験結果 #261L
                    data261L.Add(data);
                }
            }

            string fileName;
            string filePath;
            var pw = string.Empty;

            if (data101Mikasa.Count > 0)
            {   // 試験結果 #101 (Mikasa)
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_MIKASA}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_MIKASA}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr101Mikasa;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data101Mikasa, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data101Mikasa)
                {   // Fixbug #10082
                    //var result = Constant.File_Exist.FirstOrDefault(d => d.File.Name == data.File.Name);
                    //if (result != null) continue;
                    // Fixbug #10104
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data101Sekisui.Count > 0)
            {   // 試験結果 #101 (Sekisui)
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_SEKISUI}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_SEKISUI}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr101Sekisui;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data101Sekisui, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data101Sekisui)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data101Yazaki.Count > 0)
            {   // 試験結果 #101 (Yazaki)
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_YAZAKI}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_YAZAKI}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr101Yazaki;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data101Yazaki, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data101Yazaki)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data101.Count > 0)
            {   // 試験結果 #101
                //fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101}.xlsx";
                fileName = $"{Constant.TEST_RESULT}#101.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}#101.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                // TODO: fileName = data101[0].Excel;
                //var pw = Settings.Default.Tr101;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data101, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data101)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data102.Count > 0)
            {   // 試験結果 #102
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_102}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_102}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr102;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data102, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data102)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data103.Count > 0)
            {   // 試験結果 #103
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_103}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_103}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr103;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data103, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data103)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data106U.Count > 0)
            {   // 試験結果 #106U
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_106U}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_106U}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr106U;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data106U, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data106U)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data134.Count > 0)
            {   // 試験結果 #134
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_134}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_134}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr134;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data134, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data134)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data230W.Count > 0)
            {   // 試験結果 #230W
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_230W}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_230W}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr230W;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data230W, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data230W)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data232W.Count > 0)
            {   // 試験結果 #232W
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_232W}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_232W}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr232W;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data232W, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data232W)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data234W.Count > 0)
            {   // 試験結果 #234W
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_234W}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_234W}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr234W;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data234W, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data234W)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data246W.Count > 0)
            {   // 試験結果 #246W
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_246W}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_246W}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr246W;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data246W, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data246W)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data248WS.Count > 0)
            {   // 試験結果 #248WS
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_248WS}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_248WS}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr248WS;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data248WS, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data248WS)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data258WS.Count > 0)
            {   // 試験結果 #258WS
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_258WS}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_258WS}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr258WS;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data258WS, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data258WS)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data260W.Count > 0)
            {   // 試験結果 #260W
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_260W}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_260W}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr260W;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data260W, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data260W)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            if (data261L.Count > 0)
            {   // 試験結果 #261L
                fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_261L}.xlsx";
                filePath = Path.Combine(folderPath, fileName);
                if (!File.Exists(filePath))
                {
                    fileName = $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_261L}.xlsm";
                    filePath = Path.Combine(folderPath, fileName);
                }
                //var pw = Settings.Default.Tr261L;
                exportState = TestResultInterop.SetExcelCellsAsync(filePath, pw, data261L, column);
                GC.Collect();
                //if (exportState == (int)ExportState.OK)
                foreach (var data in data261L)
                {
                    if (data.ExportState != ExportState.OK) continue;
                    //data.File.Delete();
                    Utility.BackupFileCSV(data.File);
                }
                //else return exportState;
            }
            //isSuccess = true;
            //catch (Exception){}
            return exportState;
        }

        public static string GetExcelPath(string folderPath, FileInfo file, string extension = ".xlsx")
        {
            var filePath = "";

            var basename = Path.GetFileNameWithoutExtension(file.Name);
            //basename = basename.Trim();
            basename = basename.Replace(" ", "");
            basename = basename.Replace("(", "");
            basename = basename.Replace(")", "");
            basename = basename.Replace("（", "");
            basename = basename.Replace("）", "");
            basename = basename.ToUpper();

            if (basename.Contains(Constant.TEST_RESULT_101_MIKASA_SHORT))
            {   // 試験結果 #101 (Mikasa)
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_MIKASA}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_101_SEKISUI_SHORT))
            {   // 試験結果 #101 (Sekisui)
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_SEKISUI}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_101_YAZAKI_SHORT))
            {   // 試験結果 #101 (Yazaki)
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_101_YAZAKI}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_101))
            {   // 試験結果 #101
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}#101{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_102))
            {   // 試験結果 #102
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_102}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_103))
            {   // 試験結果 #103
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_103}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_106U))
            {   // 試験結果 #106U
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_106U}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_134))
            {   // 試験結果 #134
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_134}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_230W))
            {   // 試験結果 #230W
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_230W}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_232W))
            {   // 試験結果 #232W
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_232W}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_234W))
            {   // 試験結果 #234W
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_234W}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_246W))
            {   // 試験結果 #246W
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_246W}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_248WS))
            {   // 試験結果 #248WS
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_248WS}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_258WS))
            {   // 試験結果 #258WS
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_258WS}{extension}");
            }
            //else if (basename.Contains(Constant.TEST_RESULT_260W) || basename.Contains(Constant.TEST_RESULT_260W_SHORT))
            else if (basename.Contains(Constant.TEST_RESULT_260W))
            {   // 試験結果 #260W
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_260W}{extension}");
            }
            else if (basename.Contains(Constant.TEST_RESULT_261L))
            {   // 試験結果 #261L
                filePath = Path.Combine(folderPath, $"{Constant.TEST_RESULT}{Constant.TEST_RESULT_261L}{extension}");
            }

            //if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            //message = $"Tệp {filePath} không tồn tại!";
            return filePath;
        }

        public static bool GetFilenameRule(FileInfo file, ref string colorValid, int rulecase = 1, string color = "")
        {
            var hasCase = false;
            var case1 = false;
            var case2 = string.Empty;
            var case3 = string.Empty;
            var case4 = false;

            var basename = Path.GetFileNameWithoutExtension(file.Name);
            //basename = basename.Trim();
            basename = basename.Replace(" ", "");
            basename = basename.Replace("(", "");
            basename = basename.Replace(")", "");
            basename = basename.Replace("（", "");
            basename = basename.Replace("）", "");
            basename = basename.ToUpper();

            if (basename.Contains(Constant.TEST_RESULT_101_MIKASA_SHORT))
            {   // 試験結果 #101 (Mikasa)
                case1 = Settings.Default.Tr101MikasaCase1;
                case2 = Settings.Default.Tr101MikasaCase2;
                case3 = Settings.Default.Tr101MikasaCase3;
                case4 = Settings.Default.Tr101MikasaCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_101_SEKISUI_SHORT))
            {   // 試験結果 #101 (Sekisui)
                case1 = Settings.Default.Tr101SekisuiCase1;
                case2 = Settings.Default.Tr101SekisuiCase2;
                case3 = Settings.Default.Tr101SekisuiCase3;
                case4 = Settings.Default.Tr101SekisuiCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_101_YAZAKI_SHORT))
            {   // 試験結果 #101 (Yazaki)
                case1 = Settings.Default.Tr101YazakiCase1;
                case2 = Settings.Default.Tr101YazakiCase2;
                case3 = Settings.Default.Tr101YazakiCase3;
                case4 = Settings.Default.Tr101YazakiCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_101))
            {   // 試験結果 #101
                case1 = Settings.Default.Tr101Case1;
                case2 = Settings.Default.Tr101Case2;
                case3 = Settings.Default.Tr101Case3;
                case4 = Settings.Default.Tr101Case4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_102))
            {   // 試験結果 #102
                case1 = Settings.Default.Tr102Case1;
                case2 = Settings.Default.Tr102Case2;
                case3 = Settings.Default.Tr102Case3;
                case4 = Settings.Default.Tr102Case4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_103))
            {   // 試験結果 #103
                case1 = Settings.Default.Tr103Case1;
                case2 = Settings.Default.Tr103Case2;
                case3 = Settings.Default.Tr103Case3;
                case4 = Settings.Default.Tr103Case4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_106U))
            {   // 試験結果 #106U
                case1 = Settings.Default.Tr106UCase1;
                case2 = Settings.Default.Tr106UCase2;
                case3 = Settings.Default.Tr106UCase3;
                case4 = Settings.Default.Tr106UCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_134))
            {   // 試験結果 #134
                case1 = Settings.Default.Tr134Case1;
                case2 = Settings.Default.Tr134Case2;
                case3 = Settings.Default.Tr134Case3;
                case4 = Settings.Default.Tr134Case4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_230W))
            {   // 試験結果 #230W
                case1 = Settings.Default.Tr230WCase1;
                case2 = Settings.Default.Tr230WCase2;
                case3 = Settings.Default.Tr230WCase3;
                case4 = Settings.Default.Tr230WCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_232W))
            {   // 試験結果 #232W
                case1 = Settings.Default.Tr232WCase1;
                case2 = Settings.Default.Tr232WCase2;
                case3 = Settings.Default.Tr232WCase3;
                case4 = Settings.Default.Tr232WCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_234W))
            {   // 試験結果 #234W
                case1 = Settings.Default.Tr234WCase1;
                case2 = Settings.Default.Tr234WCase2;
                case3 = Settings.Default.Tr234WCase3;
                case4 = Settings.Default.Tr234WCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_246W))
            {   // 試験結果 #246W
                case1 = Settings.Default.Tr246WCase1;
                case2 = Settings.Default.Tr246WCase2;
                case3 = Settings.Default.Tr246WCase3;
                case4 = Settings.Default.Tr246WCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_248WS))
            {   // 試験結果 #248WS
                case1 = Settings.Default.Tr248WSCase1;
                case2 = Settings.Default.Tr248WSCase2;
                case3 = Settings.Default.Tr248WSCase3;
                case4 = Settings.Default.Tr248WSCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_258WS))
            {   // 試験結果 #258WS
                case1 = Settings.Default.Tr258WSCase1;
                case2 = Settings.Default.Tr258WSCase2;
                case3 = Settings.Default.Tr258WSCase3;
                case4 = Settings.Default.Tr258WSCase4;
            }
            //else if (basename.Contains(Constant.TEST_RESULT_260W) || basename.Contains(Constant.TEST_RESULT_260W_SHORT))
            else if (basename.Contains(Constant.TEST_RESULT_260W))
            {   // 試験結果 #260W
                case1 = Settings.Default.Tr260WCase1;
                case2 = Settings.Default.Tr260WCase2;
                case3 = Settings.Default.Tr260WCase3;
                case4 = Settings.Default.Tr260WCase4;
            }
            else if (basename.Contains(Constant.TEST_RESULT_261L))
            {   // 試験結果 #261L
                case1 = Settings.Default.Tr261LCase1;
                case2 = Settings.Default.Tr261LCase2;
                case3 = Settings.Default.Tr261LCase3;
                case4 = Settings.Default.Tr261LCase4;
            }

            if (rulecase == 1)
            {
                hasCase = case1;
                colorValid = "Black";
            }
            else if (rulecase == 2)
            {
                if (!string.IsNullOrEmpty(case2))
                {
                    var isValid = Regex.IsMatch(color, case2, RegexOptions.IgnoreCase);
                    if (isValid)
                    {
                        var colorKey = $"-{color}-";
                        if (Utility.COLORS.ContainsKey(colorKey))
                        {
                            colorValid = Utility.COLORS[colorKey];
                        }
                    }
                    hasCase = true;
                }
            }
            else if (rulecase == 3)
            {
                if (!string.IsNullOrEmpty(case3))
                {   //string pattern = @"^(B|L|BR|GR|G|R|W|Y)$";
                    var isValid = Regex.IsMatch(color, case3, RegexOptions.IgnoreCase);
                    if (isValid)
                    {
                        var colorKey = $"-{color}-";
                        if (Utility.COLORS.ContainsKey(colorKey))
                        {
                            colorValid = Utility.COLORS[colorKey];
                        }
                    }
                    hasCase = true;
                }
            }
            else if (rulecase == 4)
            {
                hasCase = case4;
                colorValid = "Black";
            }

            return hasCase;
        }

        public static bool IsFileLocked(string filePath)
        {
            try
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}