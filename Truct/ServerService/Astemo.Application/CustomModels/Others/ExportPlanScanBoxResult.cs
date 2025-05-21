
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Astemo.Application.CustomModels.Others
{
    /// <summary>
    /// Danh sách các thùng xuất kho do handy gửi về (Màn PC052)
    /// </summary>
    public class ExportPlanScanBoxResult
    {
        public string? OrderNumber { get; set; }
        public string? CustomerCode { get; set; }
        public string? BoxSerial { get; set; }//Serial thung
        public int Quantity { get; set; }//So luong
        public string? Result { get; set; }
        public DateTime? TimeReadQrCode { get; set; }
        [Description("Người đọc mã")]
        [MaxLength(20)]
        public string? Implementer { get; set; }
        [Description("Thời gian đọc HU Serial")]
        public DateTime? TimeReadHU { get; set; }
        [Description("HU serial")]
        [MaxLength(20)]
        public string? HUSerial { get; set; }
        [Description("Chữ khắc laze")]
        [MaxLength(30)]
        public string? LaserEngraving { get; set; }
        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }
        [Description("Thời điểm xuất kho")]
        public DateTime? TimeRelease { get; set; }
        [Description("Người xuất kho")]
        [MaxLength(20)]
        public string? WarehouseReleasePerson { get; set; }
    }
}
