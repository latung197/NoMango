
using System.ComponentModel;

namespace Astemo.Application.CustomModels.Others
{
    /// <summary>
    /// Model dùng cho màn hình PC008 - Đơn hàng chuẩn bị xuất
    /// </summary>
    public class PrepareProduct
    {
        [Description("ID thùng hàng")]
        public int BoxID { get; set; }


        [Description("ID kế hoạch")]
        public int ExportPlanID {  get; set; }


        [Description("Serial của thùng hàng")]
        public string BoxSerial { get; set; }


        [Description("Mã số sản phẩm lắp cụm")]
        public string? AssemblyProductCode { get; set; }


        [Description("Mã số sản phẩm")]
        public string ProductCode { get; set; }


        [Description("Tên sản phẩm")]
        public string ProductName { get; set; }


        [Description("Số lượng")]
        public int Quantity { get; set; }


        [Description("Số lượng chỉ thị")]
        public int IndicatorQuantity { get; set; }


        [Description("Xuất rời - Xuất ASSY")]
        public int ExportType {  get; set; }


        [Description("Trạng thái thùng hàng")]      // Đã đủ, nằm trong danh sách nhưng đơn hàng đã được xuất đi, Xóa vật lý
        public int BoxState { get; set; }


        [Description("Trạng thái đơn hàng")]
        public int OrderState {  get; set; }        // TODO: remove
    }
}
