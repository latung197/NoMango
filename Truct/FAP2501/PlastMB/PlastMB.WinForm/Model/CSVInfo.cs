//==================================================================================================
// System  : DenKa
// File    : CSVInfo.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// TestInfo, ShapeInfo, DeviationInfo, ForceInfo.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using PlastMB.Common;
using PlastMB.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PlastMB.Model
{
    public class CSVInfo
    {
        public FileInfo File
        {
            get; set;
        }

        // Array: Test Name, Test Result N/10mm, Test Result N/19mm, Thickness, Width, Weight,
        // Comment, Program, Report Date, Test Date, Type Test, Speed, Disp. Origin, No of Batches, Qty/Batch.

        /*public List<BAAdhesivePlate> BAAdhesivePlates
        {
            get; set;
        }
        public List<InsulationTapeTensile> InsulationTapeTensiles
        {
            get; set;
        }*/
        public List<InsulationTapeTensile> TestResult
        {
            get; set;
        }

        public string LotNo
        {
            get; set;
        }

        public string Lot
        {
            get; set;
        }

        public string Line
        {
            get; set;
        }

        public string Color
        {
            get; set;
        }

        public string Length
        {
            get; set;
        }

        public bool NL
        {
            get; set;
        }

        public string Program
        {
            get; set;
        }

        public string Program2
        {
            get; set;
        }

        public string Excel
        {
            get; set;
        }

        public ExportState InputState
        {
            get; set;
        }

        public ExportState ExportState
        {
            get; set;
        }

        public int column
        {
            get; set;
        } = -1;

        public double GetElongationResult(int index, bool rounding = true)
        {
            double result = 0.0f;

            if (index < TestResult.Count && !String.IsNullOrEmpty(TestResult[index].TestResult2N10mm))
            {
                //foreach (var parameters in BatchsInfo[index].Parameters)
                //    if (parameters.Name == Constant.ELONGATED)
                var value = TestResult[index].TestResult2N10mm;
                //if (double.TryParse(value, out var res))
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                {
                    if (rounding)
                    {
                        if (res < 1)
                        {
                            result = Math.Round(res, 3, MidpointRounding.ToEven);
                        }
                        else
                        {
                            result = Math.Round(res, 1, MidpointRounding.ToEven);
                        }
                    }
                    else
                    {
                        result = res;
                    }
                }
            }

            return result;
        }

        public double GetForceResult(int index, string key = "", string key2 = "", bool rounding = true)
        {
            double result = 0.0f;

            if (key == Constant.DO_GIAN_ROW)
            {
                return GetElongationResult(index, rounding);
            }

            if (key2 == Constant.PLATE)
            {   // Lấy kết qua đo của Dính Bảng
                if (index > 0) return result;
                for (var i = 0; i < TestResult.Count; i++)
                {
                    if (TestResult[i].TestName != key2) continue;
                    if (TestResult[i].TestResultN10mm > 0)
                    {
                        var value = TestResult[i].TestResultN10mm;
                        /*if (Double.TryParse(value, out double res))
                        {
                            return res;
                        }*/
                        var dec = (decimal)value;
                        result = (double)dec;
                        return result;
                        // TODO: get N19mm
                    }
                }
            }
            else if (key2 == Constant.BACKYARD)
            {   // Lấy kết qua đo của Dính Lưng
                if (index > 0) return result;
                for (var i = 0; i < TestResult.Count; i++)
                {
                    if (!TestResult[i].TestName.Contains(key2)) continue;
                    if (TestResult[i].TestResultN10mm > 0)
                    {
                        var value = TestResult[i].TestResultN10mm;
                        /*if (Double.TryParse(value, out double res))
                        {
                            return res;
                        }*/
                        var dec = (decimal)value;
                        result = (double)dec;
                        return result;
                        // TODO: get N19mm
                    }
                }
            }
            //else { }

            if (index >= TestResult.Count) return result;

            var basename = Path.GetFileNameWithoutExtension(File.Name);
            basename = basename.ToUpper();
            if (basename.Contains(Constant.TEST_RESULT_260W))
            {
                return GetForceResultN19mm(index);
            }

            if (TestResult[index].TestResultN10mm > 0)
            {
                var value = TestResult[index].TestResultN10mm;
                //if (Double.TryParse(value, out double res))
                if (rounding)
                {
                    if (value < 10)
                    {
                        result = Math.Round(value, 3, MidpointRounding.ToEven);
                    }
                    else
                    {
                        result = Math.Round(value, 1, MidpointRounding.ToEven);
                    }
                }
                else
                {
                    var dec = (decimal)value;
                    result = (double)dec;
                }
            }

            return result;
        }

        public double GetForceResultN19mm(int index, bool rounding = true)
        {
            double result = 0.0f;

            if (index >= TestResult.Count) return result;

            if (!string.IsNullOrEmpty(TestResult[index].TestResultN19mm))
            {
                var value = TestResult[index].TestResultN19mm;
                //string input = value.Replace(',', '.');
                //if (double.TryParse(value, out var res))
                if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                {
                    if (rounding)
                    {
                        if (res < 10)
                        {
                            result = Math.Round(res, 3, MidpointRounding.ToEven);
                        }
                        else
                        {
                            result = Math.Round(res, 1, MidpointRounding.ToEven);
                        }
                    }
                    else
                    {
                        result = res;
                    }
                }
            }

            return result;
        }

        public double GetForceAverage(string key = "")
        {
            var list = new List<double>();

            if (key == Constant.PLATE)
            {
                for (var i = 0; i < TestResult.Count; i++)
                {
                    if (TestResult[i].TestName != key) continue;
                    if (TestResult[i].TestResultN10mm > 0)
                    {
                        var value = TestResult[i].TestResultN10mm;
                        /*if (Double.TryParse(value, out double res))
                        {
                            list.Add(res);
                        }*/
                        var dec = (decimal)value;
                        var result = (double)dec;
                        return result;
                        // TODO: get N19mm
                    }
                }
            }
            else if (key == Constant.BACKYARD)
            {
                for (var i = 0; i < TestResult.Count; i++)
                {
                    if (!TestResult[i].TestName.Contains(key)) continue;
                    if (TestResult[i].TestResultN10mm > 0)
                    {
                        var value = TestResult[i].TestResultN10mm;
                        /*if (Double.TryParse(value, out double res))
                        {
                            list.Add(res);
                        }*/
                        var dec = (decimal)value;
                        var result = (double)dec;
                        return result;
                        // TODO: get N19mm
                    }
                }
            }
            else
            {
                var basename = Path.GetFileNameWithoutExtension(File.Name);
                basename = basename.ToUpper();
                if (basename.Contains(Constant.TEST_RESULT_260W))
                {   // get N19mm
                    return GetForceAverageN19mm();
                }

                for (var i = 0; i < TestResult.Count; i++)
                {
                    if (TestResult[i].TestResultN10mm > 0)
                    {
                        var value = TestResult[i].TestResultN10mm;
                        var dec = (decimal)value;
                        var res = (double)dec;
                        //if (Double.TryParse(value, out double res))
                        list.Add(res);
                        //list.Add(value);
                    }
                }
            }
            var average = list.Count > 0 ? list.Average() : 0.0;
            var rounding = Settings.Default.Rounding;
            double rounded = Math.Round(average, rounding);
            return rounded;
        }

        public double GetForceAverageN19mm()
        {
            var list = new List<double>();

            for (var i = 0; i < TestResult.Count; i++)
            {
                if (!string.IsNullOrEmpty(TestResult[i].TestResultN19mm))
                {
                    var value = TestResult[i].TestResultN19mm;
                    //if (double.TryParse(value, out var res))
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                    {
                        list.Add(res);
                    }
                }
            }
            var average = list.Count > 0 ? list.Average() : 0.0;
            var rounding = Settings.Default.Rounding;
            double rounded = Math.Round(average, rounding);
            return rounded;
        }

        public string Key => $"LotNo: {LotNo}, Color: {Color}{(NL ? " NL" : "")}";
    }
}