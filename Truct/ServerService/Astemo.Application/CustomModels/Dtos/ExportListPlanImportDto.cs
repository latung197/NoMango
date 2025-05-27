using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Core.Application.CustomModels.Dtos
{
    [Description("Kế hoạch xuất kho")]
    public class ExportListPlanImportDto
    {
        //[ForeignKey]
        [Description("ID sản phẩm")]
        public int? ProductID { get; set; }


        [MaxLength(20)]
        [Description("Nhân viên")]
        public string? Staff { get; set; }


        [MaxLength(20)]
        [Description("Mã số đơn hàng")]
        public string? OrderNumber { get; set; }


        [MaxLength(10)]
        [Description("Số bưu kiện")]
        public string? ParcelNo { get; set; }


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


        [Description("Xuất rời - Xuất ASSY")]
        public int ExportType { get; set; } = 0;    //EnumExportType


        [MaxLength(20)]
        [Description("Nhà sản xuất")]
        public string? Manufacturer { get; set; }


        [Description("Trạng thái đơn hàng")]
        public int OrderState { get; set; } = 0;    //EnumOrderState


        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;      //EnumCommon
    }
}
