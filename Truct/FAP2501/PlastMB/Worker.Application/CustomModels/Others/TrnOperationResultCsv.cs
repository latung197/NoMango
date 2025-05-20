using CsvHelper.Configuration.Attributes;
using System.ComponentModel;

namespace Worker.Application.CustomModels.Others
{
    /// <summary>
    /// Thông tin ECU get được từ file Csv
    /// </summary>
    public class TrnOperationResultCsv
    {



        [Description("ACHIEVEMENT_REGISTRATION_DATE")]
        [Index(0)]
        public DateTime Date { get; set; }
        [Description("ACHIEVEMENT_REGISTRATION_TIME")]
        [Index(1)]
        public string Time { get; set; }
        [Description("MST_MACHINE.MACHINE_NO")]
        [Index(2)]
        public string 設備No { get; set; }
        [Description("PROCESSING_START_TIME")]
        [Index(3)]
        public string 作業開始 { get; set; }
        [Description("PROCESSING_END_TIME")]
        [Index(4)]
        public string 作業完了 { get; set; }
        [Description("PROCESSING_TIME")]
        [Index(5)]
        public string 所要時間 { get; set; }
        [Description("PROGRESS_RATE")]
        [Index(6)]
        public string 進捗率 { get; set; }
        [Description("STANDARD_TIME")]
        [Index(7)]
        public string 標準時間 { get; set; }
        [Description("PROCESSING_STOP_TIME")]
        [Index(8)]
        public string 作業停止時間 { get; set; }
        [Description("LOSS_STOP_TIME")]
        [Index(9)]
        public string ロス停止時間 { get; set; }
        [Description("SLIP_NO")]
        [Index(10)]
        public string 伝票No { get; set; }
        [Description("CUSTOMER_NAME")]
        [Index(11)]
        public string 得意先名 { get; set; }
        [Description("DUE_DATE")]
        [Index(12)]
        public string 納期 { get; set; }
        [Description("PRODUCT_NAME_1")]
        [Index(13)]
        public string 品名1 { get; set; }
        [Description("PRODUCT_NAME_2")]
        [Index(14)]
        public string 品名2 { get; set; }
        [Description("VALUE")]
        [Index(15)]
        public string 個数 { get; set; }
        [Description("OPERATOR_1")]
        [Index(16)]
        public string オペレータ1 { get; set; }
        [Description("OPERATOR_2")]
        [Index(17)]
        public string オペレータ2 { get; set; }
        [Description("OPERATOR_3")]
        [Index(18)]
        public string オペレータ3 { get; set; }
        [Description("MEASUREMENT_INSPECTION")]
        [Index(19)]
        public string 測定検査 { get; set; }
        [Description("CHANGEOVER")]
        [Index(20)]
        public string 段取替え { get; set; }
        [Description("CAD")]
        [Index(21)]
        public string CAD { get; set; }
        [Description("EQUIPMENT_FAILURE")]
        [Index(22)]
        public string 設備故障 { get; set; }
        [Description("CLEANING")]
        [Index(23)]
        public string 清掃 { get; set; }

        [Description("REST_TIME")]
        [Index(24)]
        public string 休憩時間 { get; set; }
        

    }
}
