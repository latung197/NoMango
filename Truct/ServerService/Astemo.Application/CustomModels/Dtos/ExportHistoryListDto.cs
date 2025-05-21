using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Astemo.Application.CustomModels.Dtos
{
    [Description("Chi tiết lịch sử xuất kho xuất Nội")]
    public class ExportHistoryListDto
    {
        [Description("ID")]
        public int ExportHistoryID { get; set; }
        [Description("ID kế hoạch")]
        public int ExportPlanID { get; set; }
        [Description("Thời gian đọc mã QR")]
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
        [Description("Đối chiếu")]
        [MaxLength(10)]
        public string? Compare { get; set; }
        [Description("Số lượng")]
        public int Quantity { get; set; }
        [Description("Serial của thùng")]
        [MaxLength(100)]
        public string BoxSerial { get; set; }
        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }
        [Description("Thời điểm xuất kho")]
        public DateTime? TimeRelease { get; set; }
        [Description("Người xuất kho")]
        [MaxLength(20)]
        public string? WarehouseReleasePerson { get; set; }
        [Description("Đã xuất ok chưa")]
        public int Exported { get; set; }//EnumCommon. 0 is not exported, 1 is exported
        [Description("Trạng thái đơn hàng")]
        public int OrderState { get; set; }//EnumOrderState
        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;//EnumCommon
    }

    [Description("Chi tiết lịch sử xuất kho xuất Nội")]
    public class ExportHistoryPlanListDto
    {
        [Description("ID kế hoạch")]
        public int ExportPlanID { get; set; }


        [Description("ID")]
        public int ExportHistoryID { get; set; }


        [Description("Thời gian đọc mã QR")]
        public DateTime? TimeReadQrCode { get; set; }


        [Description("Người đọc mã")]
        [MaxLength(20)]
        public string? Implementer { get; set; }


        [MaxLength(20)]
        [Description("Mã số đơn hàng")]
        public string? OrderNumber { get; set; }


        [MaxLength(10)]
        [Description("Số bưu kiện")]
        public string? ParcelNo { get; set; }


        [Description("Thông tin tham chiếu")]
        public int ReferenceType { get; set; } = 0;     //Enum reference type


        [Description("Xuất rời - Xuất ASSY")]
        public int ExportType { get; set; } = 0;        //EnumExportType


        [MaxLength(20)]
        [Description("Mã khách hàng")]
        public string? CustomerCode { get; set; }


        [MaxLength(20)]
        [Description("Khách hàng")]
        public string? Customer { get; set; }


        [MaxLength(15)]
        [Description("Mã kế hoạch")]
        public string? PlanCode { get; set; }


        [Description("Ngày chỉ thị giao hàng")]
        public DateTime? DeliveryDate { get; set; }


        [Description("Số lượng chỉ thị")]
        public int IndicatorQuantity { get; set; }


        [Description("Số lượng hộp đựng")]
        public int BoxNo { get; set; }


        [MaxLength(15)]
        [Description("Mã bản vẽ")]
        public string? DrawingCode { get; set; }


        [MaxLength(30)]
        [Description("Mã số sản phẩm lắp cụm")]
        public string? AssemblyProductCode { get; set; }


        [MaxLength(20)]
        [Description("Mã số sản phẩm")]
        public string? ProductCode { get; set; }


        [MaxLength(50)]
        [Description("Tên sản phẩm")]
        public string? ProductName { get; set; }


        [MaxLength(10)]
        [Description("Dấu nhận dạng")]
        public string? IdentityMark { get; set; }


        [MaxLength(20)]
        [Description("Nhà sản xuất")]
        public string? Manufacturer { get; set; }


        [Description("Thời gian đọc HU Serial")]
        public DateTime? TimeReadHU { get; set; }


        [Description("HU serial")]
        [MaxLength(20)]
        public string? HUSerial { get; set; }


        [Description("Chữ khắc laze")]
        [MaxLength(30)]
        public string? LaserEngraving { get; set; }


        [Description("Đối chiếu")]
        [MaxLength(10)]
        public string? Compare { get; set; }


        [Description("Số lượng")]
        public int Quantity { get; set; }


        [Description("Serial của thùng")]
        [MaxLength(100)]
        public string? BoxSerial { get; set; }


        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }


        [Description("Thời điểm xuất kho")]
        public DateTime? TimeRelease { get; set; }


        [Description("Người xuất kho")]
        [MaxLength(20)]
        public string? WarehouseReleasePerson { get; set; }


        [Description("Đã xuất ok chưa")]
        public int Exported { get; set; }       //EnumCommon. 0 is not exported, 1 is exported


        [Description("Trạng thái đơn hàng")]
        public int OrderState { get; set; }     //EnumOrderState


        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;  //EnumCommon
    }
}
