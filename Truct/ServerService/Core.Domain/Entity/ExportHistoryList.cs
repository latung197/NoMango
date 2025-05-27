using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Core.Domain.Abstractions;
using System.ComponentModel;

namespace Core.Domain.Entity
{
    [Description("Lịch sử xuất kho")]
    [Table(name: "export_history_list", Schema = "public")]
    public class ExportHistoryList : AuditableImpl
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("ID")]
        public int ExportHistoryID { get; set; }


        // ----------------
        // Thông tin đơn hàng của sản phẩm

        [Description("ID kế hoạch")]
        public int ExportPlanID { get; set; }

        [Description("Đã xuất ok chưa")]
        public int Exported { get; set; }       //EnumCommon. 0 is not exported, 1 is exported


        // ----------------
        // Thông tin của Thùng Hàng chứa sản phẩm

        [Description("Serial của thùng")]
        [MaxLength(100)]
        public string BoxSerial { get; set; }


        [Description("Thời gian đọc mã QR")]        // TODO: move to boxinfo
        public DateTime? TimeReadQrCode { get; set; }


        [Description("Người đọc mã")]
        [MaxLength(20)]
        public string? Implementer { get; set; }    // Người nhập kho

        [Description("Số lượng")]
        public int Quantity { get; set; }


        // ----------------
        // Thông tin Sản Phẩm

        //[ForeignKey]
        [Description("HU serial")]
        [MaxLength(20)]
        public string? HUSerial { get; set; }


        [Description("Thời gian đọc HU Serial")]
        public DateTime? TimeReadHU { get; set; }


        [Description("Chữ khắc laze")]
        [MaxLength(30)]
        public string? LaserEngraving { get; set; }


        [Description("Đối chiếu")]
        [MaxLength(10)]
        public string? Compare { get; set; }


        [Description("Ca làm việc")]
        public int ShiftWork { get; set; }      // Ca làm việc nhập kho


        //[Description("Số lượng")]
        //public int Quantity { get; set; } = 1


        [Description("Thời điểm xuất kho")]
        public DateTime? TimeRelease { get; set; }


        [Description("Người xuất kho")]
        [MaxLength(20)]
        public string? WarehouseReleasePerson { get; set; }


        [Description("Trạng thái đơn hàng => Trạng thái thùng hàng")]
        public int OrderState { get; set; }     //EnumOrderState    TODO: move to boxinfo


        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;  //EnumCommon
    }
}
