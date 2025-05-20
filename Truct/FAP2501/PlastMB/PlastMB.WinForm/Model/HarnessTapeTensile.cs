//==================================================================================================
// System  : DenKa
// File    : HarnessTapeTensile.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No. ③ Harness Tape Tensile ( EA_Lực kéo )
// - Old: 3. KEO DUT, GIAN HT.csv
// - New: HT_[3] luc keo
//
// Test Name, Tensile N/10mm, Tensile N/19mm, Elongation N/10mm, Elongation N/19mm, Thickness, Width,
// Weight, Comment, Program, Report Date, Test Date, Type Test, Speed, Disp. Origin, No of Batches, Qty/Batch.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System.Globalization;

namespace PlastMB.Model
{
public class HarnessTapeTensile
{
    // TestName, TensileN/10mm, TensileN/19mm, ElongationN/10mm, ElongationN/19mm, Thickness, Width, Weight, Comment, Program, ReportDate, TestDate, TypeTest, Speed, Disp.Origin, NoofBatches, Qty/Batch.
    // Name, Max_Force_10mm_width, Max_Force_original_width, Force_at_100_percent_10mm_width, modulus_at_100_percent, Thickness, Width, Weight, Comment1, Test Name, Report Date, Test Date, Test Type, Speed, Disp. Origin, No of Batches:, Qty/Batch:
    // 1_1,13.4,25.5,-.-,-.-,1.0000,19.0000,100.0000,,,,harness tape tensile test.xmak,7/13/2024,7/13/2024,Single,Tensile,300mm/min,Plate,1,3
    // 1_2,13.2,25.1,-.-,-.-,1.0000,19.0000,100.0000,,,,harness tape tensile test.xmak,7/13/2024,7/13/2024, Single, Tensile,300mm/min, Plate,1,3


    // - Name - : First, plate, backyard of the, 1 _ 1, 1 _ 2, 1 _ 3, 1 _ 4, 1 _ 5
    // 2 đặc tính sẽ trong cùng 1 file
    // Kết quả đo Cột B + Cột D

    public string TestName
    {   // Column A
        get; set;
    }

    // - ForceInfo -
    // insulation tape tensile test
    // "Name", "Max_Force_10mm_width", "Max_Force_original_width", "Force_at_100_percent_10mm_width", "modulus_at_100_percent"
    // "Unit", "N/10mm", "N", "N/10mm", "N/mm2"
    // "1 _ 1", "21.2", "40.2", "-.-", "-.-"
    // "1 _ 2", "23.0", "43.7", "-.-", "-.-"

    public string TestResultN10mm
    {   // Column B: Max_Force_10mm_width
        get; set;
    }

    public string TestResultN19mm
    {   // Column C: N/10mm2
        get; set;
    }

    public string TestResult2N10mm
    {   // Column D: elongation
        get; set;
    }

    public string TestResult2N19mm
    {   // Column E: Max_Force_original_width
        get; set;
    }


    // "Name", "Thickness", "Width", "Gauge_Length"
    // "Size Unit:", "mm", "mm", "mm"
    // "1 _ 1", "1.0000", "19.0000", "100.0000"
    // "1 _ 2", "1.0000", "19.0000", "100.0000"

    public string Thickness
    {   // Column F: Thickness
        get; set;
    }

    public string Width
    {   // Column G
        get; set;
    }

    public string Weight
    {   // Column H: Gauge_Length
        get; set;
    }

    // - TestInfo -

    public string Comment
    {   // Column I
        get; set;
    }

    public string KeyWord
    {   // Column J
        get; set;
    }

    public string ProductName
    {   // Column K
        get; set;
    }

    public string Program
    {   // Column H: Method File Name
        get; set;
    }

    public string ReportDate
    {
        get; set;
    }

    public string TestDate
    {
        get; set;
    }

    public string TestMode
    {
        get; set;
    }

    public string TypeTest
    {
        get; set;
    }

    public string Speed
    {
        get; set;
    }

    public string DispOrigin
    {
        get; set;
    }

    public int NoBatches
    {
        get; set;
    }

    public int QtyBatch
    {
        get; set;
    }

    public InsulationTapeTensile GetFullResult()
    {
        float testResult;
        //float.TryParse(TestResultN10mm, out testResult);
        float.TryParse(TestResultN10mm, NumberStyles.Float, CultureInfo.InvariantCulture, out testResult);

        InsulationTapeTensile result = new InsulationTapeTensile
        {
            TestName = TestName,
            TestResultN10mm = testResult,
            TestResultN19mm = TestResultN19mm,
            TestResult2N10mm = TestResult2N10mm,
            TestResult2N19mm = TestResult2N19mm,
            Thickness = Thickness,
            Width = Width,
            Weight = Weight,
            Comment = Comment,
            Program = Program,
            ReportDate = ReportDate,
            TestDate = TestDate,
            TypeTest = TypeTest,
            Speed = Speed,
            DispOrigin = DispOrigin,
            NoBatches = NoBatches,
            QtyBatch = QtyBatch
        };
        return result;
    }
}
}