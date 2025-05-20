//==================================================================================================
// System  : DenKa
// File    : BAAdhesivePlate.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No. ② BA Adhesive Plate ( EA_Dính bảng + EA_Dính lưng )
// - Old: 4. LUC DINH BANG, LUNG EA.csv
// - New: EA_[2] luc dinh bang ,  [2] dinh lung
// 
// Test Name, Test Result N/10mm, Test Result N/19mm, Thickness, Width, Weight, Comment,
// Program, Report Date, Test Date, Type Test, Speed, Disp. Origin, No of Batches, Qty/Batch.
//
// TODO::
// No. ⑥ Wrap joint adhesion test (HT_Xếp chồng)
// - Old: 6. XEP CHONG HT.csv
// - New: HT_[6] luc dinh xep chong
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System.Globalization;

namespace PlastMB.Model
{
public class BAAdhesivePlate
{
    // TestName, TestResultN/10mm, TestResultN/19mm, Thickness, Width, Weight, Comment, Program, ReportDate, TestDate, TypeTest, Speed, Disp.Origin, NoofBatches, Qty/Batch.
    // Name, Average 10mm width, Peak Max 10mm width, Valley Min 10mm width, Width, Weight, Comment1, Test Name, Report Date, Test Date, Test Type, Speed, Disp. Origin, No of Batches:, Qty/Batch:
    // plate,0.86,-.-,-.-,19.0000,1.0000,,BAadhesiveplate.xmak,7/13/2024,7/13/2024,Peel,300mm/min,Force0.3N,2,1
    // backyard of the,1.40,-.-,-.-,19.0000,1.0000,,BA adhesive plate.xmak,7/13/2024,7/13/2024,Peel,300mm/min,Force 0.3N,2,1


    // - Name - : First, plate, backyard of the, 1 _ 1, 1 _ 2, 1 _ 3, 1 _ 4, 1 _ 5
    // 2 đặc tính sẽ trong cùng 1 file
    // "Plate": Dính bảng
    // "backyard of the": Dính lưng

    public string TestName
    {   // Column A
        get; set;
    }

    // - ForceInfo -
    // "Name", "Average 10mm width", "Peak Max 10mm width", "Valley Min 10mm width"
    // "Unit", "N/10mm", "N/10mm", "N/10mm"
    // "plate","19.0000", "1.0000"
    // "backyard of the", "19.0000", "1.0000"

    public string TestResultN10mm
    {   // Column B: Average 10mm width
        get; set;
    }

    public string TestResultN19mm
    {   // Column C: Peak Max 10mm width
        get; set;
    }

    public string Thickness
    {   // TODO:: Valley Min 10mm width
        get; set;
    }

    // "Name", "Width", "Weight"
    // "Size Unit:", "mm", "gf"
    // "plate", "19.0000", "1.0000"
    // "backyard of the", "19.0000", "1.0000"

    public string Width
    {   // Column E
        get; set;
    }

    public string Weight
    {   // Column F
        get; set;
    }

    // - TestInfo -

    public string Comment
    {   // Column G
        get; set;
    }

    public string Program
    {   // Column H
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