//==================================================================================================
// System  : DenKa
// File    : CsvAction.cs
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
internal class CsvAction
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
                        testResult.Add(csv.GetRecord<InsulationTapeTensile>());
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

            var fileName = Path.GetFileNameWithoutExtension(file.Name) + "-";
            var extension = Path.GetExtension(fileName);
            if (extension.Length > 0 && extension.Length <= 5)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + "-";
            }
            if (fileName.Contains("-NL-"))
            {
                testInfo.NL = true;
                fileName = fileName.Replace("-NL-", "-");
            }
            else if (fileName.Contains(" NL-"))
            {
                testInfo.NL = true;
                fileName = fileName.Replace(" NL-", "-");
            }
            else
            {
                testInfo.NL = false;
            }

            foreach (var color in Utility.COLORS)
            {
                if (fileName.Contains(color.Key))
                {
                    testInfo.Color = color.Value;
                    fileName = fileName.Replace(color.Key, "-");
                    break;
                }
            }

            var parts = fileName.Split('_');
            string[] keys;
            if (parts.Length > 0)
            {
                keys = parts[parts.Length - 1].Split('-');
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

            // Case 1 【LOT】
            if (keys.Length == 0)
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

            if (Int32.TryParse(testInfo.Line, out int j))
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

            if (keys.Length == 3 && keys[2] == string.Empty && string.IsNullOrEmpty(testInfo.Color))
            {
                testInfo.Color = "Black";
            }

            if (string.IsNullOrEmpty(testInfo.Color))
            {
                testInfo.InputState = ExportState.WrongColor;
                testInfo.ExportState = ExportState.WrongColor;
                goto L_END;
            }

            // Case 2 【LOT】-【Color】
            // Color was cut off at Line #137

            // Case 4 【LOT】-【NL】
            // NL was cut off at Line #120

            // Case 3 【LOT】-【Color】-【Length】
            // TODO: set in config @"^[1-9][0-9][MY]$" !?

        L_END:
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