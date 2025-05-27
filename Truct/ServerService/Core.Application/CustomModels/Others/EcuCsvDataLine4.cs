using CsvHelper.Configuration.Attributes;
using System.ComponentModel;

namespace Core.Application.CustomModels.Others
{
    /// <summary>
    /// Thông tin ECU get được từ file Csv
    /// </summary>
    public class EcuCsvDataLine4
    {
        [Description("Ngày giờ")]
        [Index(0)]//Index column in file CSv
        public DateTime? DateManufacture { get; set; }

        [Index(1)]
        [Description("Mã HU")]
        public string? HUCode { get; set; }
        [Index(2)]
        [Description("In laze")]
        public string LaserPrinting { get; set; }
        [Index(3)]
        public string ReproductionDetermination {  get; set; }
    }
}
