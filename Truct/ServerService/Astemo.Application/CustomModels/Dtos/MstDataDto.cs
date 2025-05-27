using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Core.Application.CustomModels.Dtos
{
    public class MstDataDto
    {
        [Description("ProductId")]
        public int ProductID { get; set; }
        [Description("Khách hàng")]
        [MaxLength(20)]
        public string? Customer { get; set; }
        [Description("Trạng thái xuất rời trong nước")]
        public int ExportLooseDomestic { get; set; }
        [Description("Trạng thái xuất rời xuất khẩu")]
        public int ExportLooseInternational { get; set; }
        [Description("Trạng thái xuất ASSY")]
        public int ExportAssy { get; set; }
        [MaxLength(100)]
        [Description("Tên sản phẩm xuất rời")]
        public string? ExportLooseName { get; set; }
        [MaxLength(100)]
        [Description("Tên sản phẩm xuất ASSY")]
        public string? ExportAssyName { get; set; }
        [Description("Mã số sản phẩm lắp cụm")]
        [MaxLength(30)]
        public string? AssemblyProductCode { get; set; }
        [Description("Mã số cơ sở tem")]
        [MaxLength(20)]
        public string? StampCode { get; set; }
        [Description("Ngày ban hành tem")]
        public DateTime? StampReleaseDt { get; set; }
        [Description("Quy cách đóng gói cơ bản")]
        public int PackageAmount { get; set; }
        [Description("Chủng loại")]
        [MaxLength(10)]
        public string? Type { get; set; }
        [Description("Mã số nhận dạng HU")]
        [MaxLength(10)]
        public string? HuSerial { get; set; }
        [Description("Mã số bản vẽ nội bộ")]
        [Required]
        [MaxLength(20)]
        public string? InternalDrawingCode { get; set; }
        [Description("Mã số sản phẩm")]
        [Required]
        [MaxLength(20)]
        public string? ProductCode { get; set; }
        [Description("Dấu nhận dạng")]
        [Required]
        [MaxLength(10)]
        public string? IdentityMark { get; set; }
        [Description("ID xác thực")]
        [MaxLength(10)]
        public string? ValidationId { get; set; }
        [Description("Thông tin tham chiếu")]
        public int ReferenceType { get; set; } = 0;//Enum reference type
        [Description("Valid flag")]
        public int ValidFlg { get; set; } = 1;//EnumCommon
    }
}
