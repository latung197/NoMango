//==================================================================================================
// System  : DenKa
// File    : TestInfoMap.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No. ② BA Adhesive Plate
// Test Name, Test Result N/10mm, Test Result N/19mm, Thickness, Width, Weight, Comment,
// Program, Report Date, Test Date, Type Test, Speed, Disp. Origin, No of Batches, Qty/Batch.
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using CsvHelper.Configuration;
using PlastMB.Model;

namespace PlastMB.Map
{
/// <summary>
/// No. ② BA Adhesive Plate ( EA_Dính bảng + EA_Dính lưng )
/// - Old: 4. LUC DINH BANG, LUNG EA.csv
/// - New: EA_[2] luc dinh bang ,  [2] dinh lung
/// 
/// No. ⑥ Wrap joint adhesion test (HT_Xếp chồng)
/// - Old: 6. XEP CHONG HT.csv
/// - New: HT_[6] luc dinh xep chong
/// 
/// Column-count = 15
/// </summary>
public sealed class TestInfoMap : ClassMap<BAAdhesivePlate>
{
    public TestInfoMap()
    {
        Map(m => m.TestName).Index(0);
        Map(m => m.TestResultN10mm).Index(1);
        Map(m => m.TestResultN19mm).Index(2);
        Map(m => m.Thickness).Index(3);
        Map(m => m.Width).Index(4);
        Map(m => m.Weight).Index(5);
        Map(m => m.Comment).Index(6);
        Map(m => m.Program).Index(7);
        Map(m => m.ReportDate).Index(8);
        Map(m => m.TestDate).Index(9);
        Map(m => m.TypeTest).Index(10);
        Map(m => m.Speed).Index(11);
        Map(m => m.DispOrigin).Index(12);
        Map(m => m.NoBatches).Index(13);
        Map(m => m.QtyBatch).Index(14);
    }
}

/// <summary>
/// No. ③ Harness Tape Tensile ( EA_Lực kéo )
/// - Old: 3. KEO DUT, GIAN HT.csv
/// - New: HT_[3] luc keo
/// 
/// Column-count = 20
/// </summary>
public sealed class HarnessTapeTensileMap : ClassMap<HarnessTapeTensile>
{
    public HarnessTapeTensileMap()
    {
        Map(m => m.TestName).Index(0);
        Map(m => m.TestResultN10mm).Index(1);
        Map(m => m.TestResultN19mm).Index(2);
        Map(m => m.TestResult2N10mm).Index(3);
        Map(m => m.TestResult2N19mm).Index(4);
        Map(m => m.Thickness).Index(5);
        Map(m => m.Width).Index(6);
        Map(m => m.Weight).Index(7);
        Map(m => m.Comment).Index(8);
        Map(m => m.KeyWord).Index(9);
        Map(m => m.ProductName).Index(10);
        Map(m => m.Program).Index(11);
        Map(m => m.ReportDate).Index(12);
        Map(m => m.TestDate).Index(13);
        Map(m => m.TestMode).Index(14);
        Map(m => m.TypeTest).Index(15);
        Map(m => m.Speed).Index(16);
        Map(m => m.DispOrigin).Index(17);
        Map(m => m.NoBatches).Index(18);
        Map(m => m.QtyBatch).Index(19);
    }
}

/// <summary>
/// No. ④ Insulation Tape Tensile ( EA_Lực kéo + EA_Độ giãn )
/// - Old: 2. KEO DUT, GIAN EA.csv
/// - New: EA_[4] luc keo , [4] do gian
/// 
/// Column-count = 17
/// </summary>
public sealed class TensileElongationMap : ClassMap<InsulationTapeTensile>
{
    public TensileElongationMap()
    {
        Map(m => m.TestName).Index(0);
        Map(m => m.TestResultN10mm).Index(1);
        Map(m => m.TestResultN19mm).Index(2);
        Map(m => m.TestResult2N10mm).Index(3);
        Map(m => m.TestResult2N19mm).Index(4);
        Map(m => m.Thickness).Index(5);
        Map(m => m.Width).Index(6);
        Map(m => m.Weight).Index(7);
        Map(m => m.Comment).Index(8);
        Map(m => m.Program).Index(9);
        Map(m => m.ReportDate).Index(10);
        Map(m => m.TestDate).Index(11);
        Map(m => m.TypeTest).Index(12);
        Map(m => m.Speed).Index(13);
        Map(m => m.DispOrigin).Index(14);
        Map(m => m.NoBatches).Index(15);
        Map(m => m.QtyBatch).Index(16);
    }
}

/// <summary>
/// No. ⑤ Low speed unwinding force test ( HT_Bóc chậm OR EA_Bóc chậm)
/// - Old: 1. BOC CHAM HT.csv, 8. BÓC CHẬM EA.csv
/// - New: HT_[5] boc cham, EA_[5] boc cham
/// 
/// TODO:: TwelvePointsPeel !?
/// No. ① 12points peel test ( HT_Bóc chậm OR EA_Bóc chậm)
// - Old: 5. LUC DINH LUNG HT.csv, 7. LỰC DÍNH BẢNG HT.csv
// - New: HT_[1] luc dinh bang, HT_[1] luc dinh lung
// 
// Column-count = 14
/// </summary>
public sealed class LowSpeedUnwindingMap : ClassMap<LowSpeedUnwinding>
{
    public LowSpeedUnwindingMap()
    {
        Map(m => m.TestName).Index(0);
        Map(m => m.TestResultN10mm).Index(1);
        Map(m => m.TestResultN19mm).Index(2);
        Map(m => m.Width).Index(3);
        Map(m => m.Weight).Index(4);
        Map(m => m.Comment).Index(5);
        Map(m => m.Program).Index(6);
        Map(m => m.ReportDate).Index(7);
        Map(m => m.TestDate).Index(8);
        Map(m => m.TypeTest).Index(9);
        Map(m => m.Speed).Index(10);
        Map(m => m.DispOrigin).Index(11);
        Map(m => m.NoBatches).Index(12);
        Map(m => m.QtyBatch).Index(13);
    }
}
}