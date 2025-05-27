
namespace Core.Application.CustomModels.Others
{
    public class ExportPlanStampInfo
    {
        public string? CustomerCode { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int IndicatorQuantity { get; set; }//So luong chi thi
        public int PackageAmount { get; set; }
        public int BoxNo { get; set; }//So luong thung hang
        public string? DrawingCode { get; set; }//Ma ban ve
        public string? AssemblyProductCode { get; set; }//Ma so san pham lap cum
        public string? AssemblyProductCodeMaster { get; set; }//Ma so san pham lap cum cua Master data
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int ExportType { get; set; }//Xuat roi - Xuat ASSY
        public int ReferenceType { get; set; }//Tham chieu
        public string? StampCode {  get; set; } //Ma so co so
        public DateTime? StampReleaseDt { get; set; }

    }
}
