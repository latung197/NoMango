using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Astemo.Domain.Abstractions;
using System.ComponentModel;

namespace Astemo.Domain.Entity
{
    [Description("Thông thin thùng hàng")]
    [Table(name: "box_info", Schema = "public")]
    public class BoxInfo : AuditableImpl
    {
        // ----------------
        // Thông tin của Thùng Hàng

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("ID")]
        public int BoxID { get; set; }


        [Description("Serial của thùng")]
        [MaxLength(100)]
        public string BoxSerial { get; set; }


        [Description("Thời gian đọc mã QR")]
        public DateTime? TimeReadQrCode { get; set; }


        [Description("Người đọc mã")]
        [MaxLength(20)]
        public string? Implementer { get; set; }


        [Description("Số lượng chỉ thị trong hộp")]
        public int IndicatorQuantity { get; set; }


        [Description("Số lượng thực tế trong hộp")]
        public int Quantity { get; set; }


        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }          // Ca làm việc xuất kho


        [Description("Trạng thái thùng hàng")]      // Đã đủ, nằm trong danh sách nhưng đơn hàng đã được xuất đi, Xóa vật lý
        public int BoxState { get; set; }           // EnumBoxState


        // ----------------
        // Thông tin Đơn Hàng của thùng hàng

        //[ForeignKey]
        [Description("ID kế hoạch")]
        public int ExportPlanID { get; set; }

        //[MaxLength(20)]
        //[Description("Mã khách hàng")]
        //public string? CustomerCode { get; set; }

        [MaxLength(20)]
        [Description("Khách hàng")]
        public string? Customer { get; set; }

        [Description("Xuất rời - Xuất ASSY")]
        public int ExportType { get; set; } = 0;    // EnumExportType

        //[Description("Đã xuất ok chưa")]
        //public int Exported { get; set; }         // EnumCommon. 0 is not exported, 1 is exported

        [Description("Trạng thái đơn hàng")]
        public int OrderState { get; set; }       // EnumOrderState: Chưa xuất, Đang xuất, Đã đủ, Đã xuất, Hủy. TODO: remove


        // ----------------
        // Thông tin Sản phẩm của Đơn Hàng

        //[ForeignKey]
        [Description("ID sản phẩm")]
        public int? ProductID { get; set; }

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


        // ----------------

        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;      // EnumCommon
    }
}
