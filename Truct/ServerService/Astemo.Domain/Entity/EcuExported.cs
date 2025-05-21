using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Astemo.Domain.Abstractions;
using System.ComponentModel;

namespace Astemo.Domain.Entity
{
    [Description("Bảng ecu data đã xuất kho")]
    [Table(name: "ecu_exported", Schema = "public")]
    public class EcuExported : AuditableImpl
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("EcuDataID")]
        public int EcuDataID { get; set; }


        [Description("Ngày giờ")]
        public DateTime? DateManufacture { get; set; }


        [Description("Mã NG")]
        [MaxLength(10)]
        public string? NgCode { get; set; }


        [Description("Ghi chú")]
        [MaxLength(200)]
        public string? Note { get; set; }


        [Description("Kết quả")]
        public int Result { get; set; }     //EnumCommon. 0 is NG, 1 is OK


        [Description("Mã HU")]
        [MaxLength(20)]
        public string? HUCode { get; set; }


        [Description("In laze")]
        [MaxLength(30)]
        public string? LaserPrinting { get; set; }


        [Description("Trạng thái sản phẩm")]    // đã đóng gói vào thùng hay chưa
        public int PackState { get; set; }      //EnumPackState


        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;      //EnumCommon
    }
}
