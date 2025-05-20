//==================================================================================================
// System  : DenKa
// File    : TwelvePointsPeel.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No. ① 12points peel test ( HT_Bóc chậm OR EA_Bóc chậm)
// - Old: 5. LUC DINH LUNG HT.csv, 7. LỰC DÍNH BẢNG HT.csv
// - New: HT_[1] luc dinh bang, HT_[1] luc dinh lung
// 
// Test Name, Test Result N/10mm, Test Result N/19mm, Width, Weight, Comment,
// Program, Report Date, Test Date, Type Test, Speed, Disp. Origin, No of Batches, Qty/Batch.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System.Globalization;

namespace PlastMB.Model
{
public class TwelvePointsPeel
{
    // TestName, TestResultN/10mm, TestResultN/19mm, Width, Weight, Comment, Program, ReportDate, TestDate, TypeTest, Speed, Disp.Origin, NoofBatches, Qty/Batch.
    // Name, Average_of_10mm_width, Average_of_original_width, Width, Weight, Comment1, Test Name, Report Date, Test Date, Test Type, Speed, Disp. Origin, No of Batches:, Qty/Batch:
    // First,2.42,4.61,19.0000,1.0000,,Low speed unwinding force test.xmak,7/13/2024,7/13/2024,Peel,300mm/min,Force 0.3N,1,1


    // - Name - : First, plate, backyard of the, 1 _ 1, 1 _ 2, 1 _ 3, 1 _ 4, 1 _ 5
    // Chỉ có Dính bảng HA/HT hoặc Dính lưng HA/HT

    public string TestName
    {   // Column A
        get; set;
    }

    // - ForceInfo -
    // "Name", "n", "n_of_original_width"
    // "Unit", "N/10mm", "N"
    // "1 _ 1", "1.35", "2.56"
    // "1 _ 2", "1.37", "2.61"

    public string TestResultN10mm
    {   // Column B: "n"
        get; set;
    }

    public string TestResultN19mm
    {   // Column C: n_of_original_width
        get; set;
    }

    /*public string Thickness
    {   // Column D
        get; set;
    }*/

    // "Name", "Width", "Weight"
    // "Size Unit:", "mm", "gf"
    // "First", "19.0000", "1.0000"

    public string Width
    {   // Column D
        get; set;
    }

    public string Weight
    {   // Column E
        get; set;
    }

    // - TestInfo -

    public string Comment
    {   // Column F
        get; set;
    }

    public string Program
    {   // Column G
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
            TestResult2N10mm = "",
            TestResult2N19mm = "",
            Thickness = "",
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