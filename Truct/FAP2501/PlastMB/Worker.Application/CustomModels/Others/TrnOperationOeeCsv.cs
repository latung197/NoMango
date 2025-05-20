using CsvHelper.Configuration.Attributes;
using System.ComponentModel;

namespace Worker.Application.CustomModels.Others
{
    /// <summary>
    /// Thông tin ECU get được từ file Csv
    /// </summary>
    public class TrnOperationOeeCsv
    {
        [Description("ACHIEVEMENT_REGISTRATION_DATE")]
        [Index(0)]
        public DateTime Date { get; set; }

        [Description("ACHIEVEMENT_REGISTRATION_TIME")]
        [Index(1)]
        public string Time { get; set; }

        [Index(2)]
        [Description("PROCESSING_TIME")]
        public string 加工時間 { get; set; }//EnumCommon. 0 is NG, 1 is OK

        [Description("PROCESSING_STOP_TIME")]
        [Index(3)]
        public string 作業停止時間 { get; set; }

        [Index(4)]
        [Description("LOSS_STOP_TIME")]
        public string? ロス停止時間 { get; set; }

        [Description("PRODUCTION_COUNT")]
        [Index(5)]
        public string 生産件数 { get; set; }
        [Description("OPERATION_RATE")]

        [Index(6)]
        public string 稼働率 { get; set; }
        [Description("EQUIPMENT_OPERATION_HOURS")]
        [Index(7)]
        public string 設備稼働時間 { get; set; }

        [Description("LOAD_TIME")]
        [Index(8)]
        public string 負荷時間 { get; set; }

        [Description("TIME_OPERATING_RATE")]
        [Index(9)]
        public string 時間稼働率 { get; set; }

    }
}
