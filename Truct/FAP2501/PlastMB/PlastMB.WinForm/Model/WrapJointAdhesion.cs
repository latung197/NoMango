//==================================================================================================
// System  : DenKa
// File    : WrapJointAdhesion.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No. ④ Insulation Tape Tensile ( EA_Lực kéo + EA_Độ giãn )
// - Old: 2. KEO DUT, GIAN EA.csv
// - New: EA_[4] luc keo , [4] do gian
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
    public class WrapJointAdhesion
    {
        // TestName, TensileN/10mm, TensileN/19mm, ElongationN/10mm, ElongationN/19mm, Thickness, Width, Weight, Comment, Program, ReportDate, TestDate, TypeTest, Speed, Disp.Origin, NoofBatches, Qty/Batch.
        // Name, Max_Force_10mm_width, N/10mm2, elongation, Max_Force_original_width, Thickness, Width, Weight, Comment1, Test Name, Report Date, Test Date, Test Type, Speed, Disp. Origin, No of Batches:, Qty/Batch:
        // 1_1,36.6842,36.6842,254,69.7,1.0000,19.0000,100.0000,,insulation tape tensile test.xmak,7/13/2024,7/13/2024,Tensile,300mm/min,Start,1,5
        // 1_2,35.8947,35.8947,255,68.2,1.0000,19.0000,100.0000,,insulation tape tensile test.xmak,7/13/2024,7/13/2024,Tensile,300mm/min,Start,1,5


        // - Name - : First, plate, backyard of the, 1 _ 1, 1 _ 2, 1 _ 3, 1 _ 4, 1 _ 5
        // 2 đặc tính sẽ trong cùng 1 file
        // Kết quả đo Cột B + Cột D

        public string TestName
        {   // Column A
            get; set;
        }

        // - ForceInfo -
        // insulation tape tensile test
        // "Name", "Max_Force_10mm_width", "N/10mm2", "elongation", "Max_Force_original_width"
        // "Unit", "N/10mm", "N/mm2", "%", "N"
        // "1 _ 1", "36.8421", "36.8421", "269", "70.0"
        // "1 _ 2", "35.6316", "35.6316", "255", "67.7"

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

        public string Program
        {   // Column J
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