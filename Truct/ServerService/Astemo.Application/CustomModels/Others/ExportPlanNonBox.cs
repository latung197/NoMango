
namespace Astemo.Application.CustomModels.Others
{
    /// <summary>
    /// Danh sách các kế hoạch chưa có thùng hàng nào
    /// </summary>
    public class ExportPlanNonBox
    {
        public int ExportPlanID { get; set; }
        public string? CustomerCode { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int IndicatorQuantity { get; set; }//So luong chi thi
        public int BoxNo { get; set; }//So luong thung hang
        public string? DrawingCode { get; set; }//Ma ban ve
        public string? AssemblyProductCode { get; set; }//Ma so san pham lap cum
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int ReferenceType { get; set; }//Tham chieu
        public int ExportType { get; set; }//Loại xuất kho
    }
}
