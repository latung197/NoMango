//==================================================================================================
// System  : DenKa
// File    : CsvUtility.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// TestInfo, ShapeInfo, Parameters, BatchsInfo, ForceInfo.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PlastMB.Common;
using PlastMB.Map;
using PlastMB.Model;
using PlastMB.Properties;
using NLog;

namespace PlastMB.Helper
{
public class CsvUtility
{
    private static readonly CsvConfiguration CsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        IgnoreBlankLines = false,
        HasHeaderRecord = false,
    };

    // create a static logger field
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static CSVInfo ReadCsv(FileInfo file, bool IsHasHeader = false, string Demiliter = ",")
    {
        var testInfo = new CSVInfo();

        // Test result
        //var testResult = new List<BAAdhesivePlate>();
        var testResult = new List<InsulationTapeTensile>();

        if (string.IsNullOrEmpty(file.FullName))
        {
            return testInfo;
        }

        try
        {
            var config = CsvConfig;
            config.Delimiter = Demiliter;

            using (var reader = new StreamReader(file.FullName))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<TestInfoMap>();
                csv.Context.RegisterClassMap<HarnessTapeTensileMap>();
                csv.Context.RegisterClassMap<TensileElongationMap>();
                csv.Context.RegisterClassMap<LowSpeedUnwindingMap>();

                //var isHeader = false;

                while (csv.Read())
                {
                    if (csv.ColumnCount == 15)
                    {
                        // No. ② BA Adhesive Plate ( EA_Dính bảng + EA_Dính lưng )
                        // No. ⑥ Wrap joint adhesion test (HT_Xếp chồng)
                        var data = csv.GetRecord<BAAdhesivePlate>();
                        testResult.Add(data.GetFullResult());
                    }
                    else if (csv.ColumnCount == 17)
                    {
                        // No. ④ Insulation Tape Tensile ( EA_Lực kéo + EA_Độ giãn )
                        //testResult.Add(csv.GetRecord<InsulationTapeTensile>());
                        var data = csv.GetRecord<WrapJointAdhesion>();
                        testResult.Add(data.GetFullResult());
                    }
                    else if (csv.ColumnCount == 14)
                    {
                        // No. ⑤ Low speed unwinding force test ( HT_Bóc chậm OR EA_Bóc chậm)
                        // No. ① 12points peel test ( HT_Bóc chậm OR EA_Bóc chậm)
                        var data = csv.GetRecord<LowSpeedUnwinding>();
                        testResult.Add(data.GetFullResult());
                    }
                    else if (csv.ColumnCount == 20)
                    {
                        // No. ③ Harness Tape Tensile ( EA_Lực kéo )
                        var data = csv.GetRecord<HarnessTapeTensile>();
                        testResult.Add(data.GetFullResult());
                    }
                }

                //testResult = csv.GetRecords<T>().ToList();
                /*if (testResult.Count > 0)
                {
                    //testInfo.BAAdhesivePlates = csv.GetRecords<BAAdhesivePlate>().ToList();
                    testInfo.TestResult = testResult;
                }
                else if (testResult2.Count > 0)
                {
                    //testInfo.InsulationTapeTensiles = csv.GetRecords<InsulationTapeTensile>().ToList();
                    testInfo.TestResult = testResult2;
                }*/
                testInfo.TestResult = testResult;
            }

            testInfo.File = file;

            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            /*var fileName = Path.GetFileNameWithoutExtension(file.Name) + "-";
            var extension = Path.GetExtension(fileName);
            if (extension.Length > 0 && extension.Length <= 5)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + "-";
            }*/

            var parts = fileName.Split('_');
            string[] keys;
            if (parts.Length > 0)
            {
                var part = parts[parts.Length - 1];
                part = part.Replace(" ", "-");
                keys = part.Split('-');
            }
            else
            {
                keys = fileName.Split('-');
            }

            if (keys.Length > 0)
            {
                testInfo.Lot = keys[0];
                testInfo.LotNo = testInfo.Lot;
            }

            // ---==== Case 0: not LOT or not Line ====---
            if (keys.Length == 0 || string.IsNullOrEmpty(testInfo.Lot))
            {
                testInfo.InputState = ExportState.FileMalformed;
                testInfo.ExportState = ExportState.FileMalformed;
                goto L_END;
            }

            if (keys.Length == 1)
            {
                testInfo.InputState = ExportState.MissLine;
                testInfo.ExportState = ExportState.MissLine;
                goto L_END;
            }

            // ---==== Case 1: 【LOT-Line】 ====---

            var match = Regex.Match(testInfo.Lot, @"^[0-9][A-L]\d{2}$");
            if (!match.Success)
            {
                testInfo.InputState = ExportState.WrongLotNo;
                testInfo.ExportState = ExportState.WrongLotNo;
                goto L_END;
            }

            //if (keys.Length > 1)
            testInfo.Line = keys[1];
            testInfo.LotNo = testInfo.Lot + "-" + testInfo.Line;

            if (testInfo.Line == "")
            {
                testInfo.InputState = ExportState.MissLine;
                testInfo.ExportState = ExportState.MissLine;
                goto L_END;
            }

            if (int.TryParse(testInfo.Line, out var j))
            {
                var lineMax = Settings.Default.LineMax;
                //if (testInfo.Line != "1" && testInfo.Line != "2")
                if (j > lineMax)
                {
                    testInfo.InputState = ExportState.WrongLine;
                    testInfo.ExportState = ExportState.WrongLine;
                    goto L_END;
                }
            }
            else
            {
                testInfo.InputState = ExportState.WrongLine;
                testInfo.ExportState = ExportState.WrongLine;
                goto L_END;
            }

            var colorValid = string.Empty;

            //if (keys.Length == 3 && keys[2] == string.Empty && string.IsNullOrEmpty(testInfo.Color))
            if (keys.Length == 2 && string.IsNullOrEmpty(testInfo.Color))
            {
                var hasCase1 = TestResultHelper.GetFilenameRule(file, ref colorValid);
                if (hasCase1)
                {   //testInfo.Color = "Black";
                    testInfo.Color = colorValid;
                }
                else
                {
                    testInfo.InputState = ExportState.WrongCase1;
                    testInfo.ExportState = ExportState.WrongCase;
                    goto L_END;
                }
            }

            // ---==== Case 2 or Case 4: 【LOT-Line】-【Color/NL】 ====---
            if (keys.Length == 3)
            {
                if (keys[2] == "NL")
                {
                    // ---==== Case 4 【LOT】-【NL】 ====---
                    var hasCase4 = TestResultHelper.GetFilenameRule(file, ref colorValid, 4);
                    if (hasCase4)
                    {   //testInfo.Color = "Black";
                        testInfo.Color = colorValid;
                        testInfo.NL = true;
                    }
                    else
                    {
                        testInfo.InputState = ExportState.WrongCase4;
                        testInfo.ExportState = ExportState.WrongCase;
                        goto L_END;
                    }
                }
                else
                {
                    // ---==== Case 2: 【LOT-Line】-【Color】 ====---
                    var color = keys[2];
                    var hasCase2 = TestResultHelper.GetFilenameRule(file, ref colorValid, 2, color);
                    if (hasCase2)
                    {   // Get color by key
                        if (!string.IsNullOrEmpty(colorValid))
                        {
                            testInfo.Color = colorValid;
                        }
                    }
                    else
                    {
                        testInfo.InputState = ExportState.WrongCase2;
                        testInfo.ExportState = ExportState.WrongCase;
                        goto L_END;
                    }
                }
            }

            // ---==== Case 3 【LOT】-【Color】-【Length】 ====---
            // TODO: set in config @"^[1-9][0-9][MY]$" !?

            if (keys.Length == 4)
            {
                var color = keys[2];

                var hasCase3 = TestResultHelper.GetFilenameRule(file, ref colorValid, 3, color);
                if (hasCase3)
                {
                    // Get color by key
                    //if (Utility.COLORS.ContainsKey(colorKey))
                    if (!string.IsNullOrEmpty(colorValid))
                    {   // testInfo.Color = Utility.COLORS[colorKey];
                        testInfo.Color = colorValid;
                    }
                    var length = keys[3];
                    var pattern = Settings.Default.Length;
                    var isValid = Regex.IsMatch(length, pattern, RegexOptions.IgnoreCase);
                    if (isValid)
                    {
                        testInfo.Length = length;
                    }
                    else
                    {
                        testInfo.InputState = ExportState.WrongLength;
                        testInfo.ExportState = ExportState.WrongLength;
                    }
                }
                else
                {
                    testInfo.InputState = ExportState.WrongCase3;
                    testInfo.ExportState = ExportState.WrongCase;
                    goto L_END;
                }
            }

            if (string.IsNullOrEmpty(testInfo.Color))
            {
                testInfo.InputState = ExportState.WrongColor;
                testInfo.ExportState = ExportState.WrongColor;
                //goto L_END;
            }

            var basename = Utility.RemoveUnicode(file.Name);
            basename = Path.GetFileNameWithoutExtension(basename).ToLower();

            //if (basename.Contains(Constant.BOC_CHAM))
            if (basename.Contains(Constant.BOC_CHAM) || basename.Contains(Constant.BOC_THAP))
            {
                // Bóc chậm -> Lực bóc tốc độ thấp
                testInfo.Program = Constant.BOC_CHAM_ROW;
            }
            if (basename.Contains(Constant.LUC_KEO) || basename.Contains(Constant.KEO_DUT))
            {
                // Kéo đứt giãn EA -> Lực kéo | KEO_DUT_GIAN_EA
                testInfo.Program = Constant.LUC_KEO_ROW;
            }
            if (basename.Contains(Constant.DO_GIAN) && !testInfo.File.Name.StartsWith("#2"))
            {
                // Kéo đứt giãn HT -> Độ giãn KEO_DUT_GIAN_HT
                testInfo.Program2 = Constant.DO_GIAN_ROW;
            }
            if (basename.Contains(Constant.DINH_BANG))
            {
                // Dính bảng -> Dính bảng
                testInfo.Program = Constant.DINH_BANG_ROW;
            }
            if (basename.Contains(Constant.DINH_LUNG) || basename.Contains(Constant.MAT_LUNG))
            {
                // Dính lưng -> Dính lưng
                if (string.IsNullOrEmpty(testInfo.Program))
                {
                    testInfo.Program = Constant.DINH_LUNG_ROW;
                }
                else
                {
                    testInfo.Program2 = Constant.DINH_LUNG_ROW;
                }
            }
            if (basename.Contains(Constant.XEP_CHONG))
            {
                // Xếp chồng -> Lực xếp chồng
                testInfo.Program = Constant.XEP_CHONG_ROW;
            }

            if (string.IsNullOrEmpty(testInfo.Program) && string.IsNullOrEmpty(testInfo.Program2))
            {
                testInfo.InputState = ExportState.FileMissType;
                testInfo.ExportState = ExportState.FileMissType;
            }

            if (keys.Length >= 5)
            {
                testInfo.InputState = ExportState.FileMalformed;
                testInfo.ExportState = ExportState.FileMalformed;
            }

         L_END:
            var filePath = TestResultHelper.GetExcelPath(Utility.FOLDER_EXCEL, file);
            if (string.IsNullOrEmpty(filePath))
            {   // Tệp csv có tên không đúng định dạng.
                // Không tìm thấy tệp Excel tương ứng
                testInfo.InputState = ExportState.ExcelMismatch;
                testInfo.ExportState = ExportState.ExcelMismatch;
                //goto L_END;
            }

            // Log TestInfo
            foreach (var result in testInfo.TestResult)      // batchsInfo
            {
                logger.Debug(result);
            }

        }
        catch (TypeConverterException ex)
        {
            logger.Error(Utility.GetExceptionInfo(ex, "CsvAction.cs"));
            return testInfo;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("because it is being used by another process."))
            {
                testInfo.InputState = ExportState.FileOpening;
            }
            logger.Error(Utility.GetExceptionInfo(ex, "CsvAction.cs"));
            return testInfo;
        }

        return testInfo;
    }
}
}