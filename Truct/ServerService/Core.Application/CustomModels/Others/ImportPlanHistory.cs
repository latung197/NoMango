using Core.Application.CustomModels.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.CustomModels.Others
{
    public class ImportPlanHistory
    {
        // Thông tin đơn hàng của thùng hàng

        //[Required]
        //public string CustomerCode { get; set; }

        public string? Customer { get; set; }

        public DateTime? DeliveryDate { get; set; } //Ngay chi thi giao hang

        public int IndicatorQuantity { get; set; }//so luong chi thi

        public int BoxNo { get; set; }//so luong thung hang

        [Required]
        public int Quantity {  get; set; } //so luong trong hop

        [Required]
        public string? ProductCode { get; set; }

        //[Required]
        public string? ProductName { get; set; }

        public string DrawingCode { get; set; } = string.Empty;//Ma ban ve

        public string AssemblyProductCode { get; set; } = string.Empty;

        public int ExportType {  get; set; }

        public int OrderState { get; set; }//Trang thai don hang


        // Thông tin của thùng hàng

        //[ForeignKey]
        [Required]
        [Description("Số Sơ ri của thùng")]
        public string BoxSerial { get; set; }


        [Description("Khi đọc mã QR")]
        public DateTime? TimeReadQrCode { get; set; }


        [Description("Người đọc mã")]
        [MaxLength(20)]
        public string? Implementer { get; set; }


        // Thông tin sản phẩm trong thùng hàng

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


        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }

    }
}
