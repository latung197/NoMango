//===================================================================================================
// System  : DenKa
// File    : Constant.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Definition of Enum and Constant
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//===================================================================================================

namespace PlastMB.Common
{
    public enum ExportState
    {
        Unknown = -99,
        //ExcelMissType = -29,
        WrongLength = -28,
        WrongCase4 = -27,
        WrongCase3 = -26,
        WrongCase2 = -25,
        WrongCase1 = -24,
        WrongCase = -23,
        MeasureInvalid = -22,
        WrongColor = -21,
        WrongLine = -20,
        MissLine = -19,
        WrongLotNo = -18,
        FileMissType = -17,
        FileMalformed = -16,
        FileOpening = -15,
        FileNotFound = -14,
        ExcelMismatch = -13,
        ExportCraft = -7,
        PassIncorrect = -6,
        LotNoNotFound = -5,
        NotFound = -4,
        Full = -3,
        Exist = -2,
        Error = -1,
        OK = 0,
        ExportCraftSuccess = 7,
    }

    public class Constant
    {
        public const string VERSION_CODE = "V1.0.1";
        public const string VERSION_DATE = "2025-05-05 10:09:01";

        public const string MenuTitle = "Menu";

        public const string BOC_CHAM = "boccham";               // Bóc chậm  -> 低速巻戻力       Lực bóc tốc độ thấp
        public const string BOC_THAP = "bocthap";               // Bóc thấp  -> 低速巻戻力       Lực bóc tốc độ thấp
        //public const string KEO_DUT = "keodut";               // Kéo đứt   -> 引張強度         Kéo đứt
        public const string KEO_DUT = "keova";                  // Kéo va    -> 引張強度         Lực kéo
        public const string LUC_KEO = "luckeo";                 // Lực kéo   -> 引張強度         Lực kéo
        public const string DO_GIAN = "dogian";                 // Độ giãn   -> 伸び             Độ giãn
        public const string DINH_BANG = "dinhbang";             // Dính bảng -> 試験板粘着力      Dính bảng
        public const string DINH_LUNG = "dinhlung";             // Dính lưng -> 自背面粘着力      Dính lưng
        public const string MAT_LUNG = "dinhmatlung";           // Dính mặt lưng -> 自背面粘着力  Dính lưng
        public const string XEP_CHONG = "xepchong";             // Xếp chồng -> 重ね合わせ粘着力   Lực xếp chồng

        public const string ROW_1 = "（ｎ＝１）";
        public const string ROW_2 = "（ｎ＝２）";
        public const string ROW_3 = "（ｎ＝３）";
        public const string ROW_4 = "（ｎ＝４）";
        public const string ROW_5 = "（ｎ＝５）";

        public const string BOC_CHAM_KEY = "低速巻戻力";             // Bóc chậm  -> 低速巻戻力　N/10mm　Lực bóc thấp
        public const string BOC_CHAM_ROW = "Lực bóc";               // Bóc chậm  -> 低速巻戻力　N/10mm　Lực bóc tốc độ thấp
        public const string LUC_KEO_ROW = "Lực kéo";                // Lực kéo   -> 引張強さ　N/10mm　Lực kéo
        public const string DO_GIAN_ROW = "Độ giãn";                // Độ giãn   -> 伸び　％　Độ giãn
        public const string DINH_BANG_ROW = "Dính bảng";            // Dính bảng -> 試験板粘着力　N/10mm　Dính bảng
        public const string DINH_LUNG_ROW = "Dính lưng";            // Dính lưng -> 自背面粘着力　N/10mm　Dính lưng
        public const string XEP_CHONG_ROW = "Lực xếp chồng";        // Xếp chồng -> 重ね合わせ粘着力　N/10mm　Lực xếp chồng

        public const string ELONGATED = "elongation";
        public const string PLATE = "plate";
        public const string BACKYARD = "backyard";

        public const string TEST_RESULT = "試験結果 ";
        public const string TEST_RESULT_101_MIKASA_SHORT = "#101MIKASA";    // 101Mikasa
        public const string TEST_RESULT_101_SEKISUI_SHORT = "#101SEKISUI";  // 101Sekisui
        public const string TEST_RESULT_101_YAZAKI_SHORT = "#101YAZAKI";    // 101Yazaki
        public const string TEST_RESULT_101_MIKASA = "#101 (Mikasa)";
        public const string TEST_RESULT_101_SEKISUI = "#101 (Sekisui)";
        public const string TEST_RESULT_101_YAZAKI = "#101 (Yazaki)";
        public const string TEST_RESULT_101 = "#101";      // #101_
        public const string TEST_RESULT_102 = "#102";
        public const string TEST_RESULT_103 = "#103";
        public const string TEST_RESULT_106U = "#106U";
        public const string TEST_RESULT_134 = "#134";
        public const string TEST_RESULT_230W = "#230W";
        public const string TEST_RESULT_232W = "#232W";
        public const string TEST_RESULT_234W = "#234W";
        public const string TEST_RESULT_246W = "#246W";
        public const string TEST_RESULT_248WS = "#248WS";
        public const string TEST_RESULT_258WS = "#258WS";
        public const string TEST_RESULT_260W = "#260W";
        public const string TEST_RESULT_260W_SHORT = "#260";
        public const string TEST_RESULT_261L = "#261L";

        public static string VARIETY = "品種";
        public static string LOTNO = "包装ロット";
        public static string COLOR = "色 Màu";
    }
}