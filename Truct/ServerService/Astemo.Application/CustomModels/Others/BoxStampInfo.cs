using Astemo.Application.CustomModels.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Astemo.Application.CustomModels.Others
{
    /// <summary>
    /// dữ liệu in ra tem dán lên thùng
    /// </summary>
    public class BoxStampInfo
    {
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
        public string? BoxSerial { get; set; }
        public int Quantity {  get; set; }
        public DateTime? StampReleaseDt {  get; set; }
        public string? StampCode {  get; set; }
        public int PackageAmount {  get; set; }//quy cach dong goi co ban
    }
}
