using System;

namespace AIOI.Models
{
    public class Material
    {
        public string MaterialCode { get; set; }      // Mã vật tư
        public string MaterialName { get; set; }      // Tên vật tư
        public string Unit { get; set; }              // Đơn vị tính
        public decimal Quantity { get; set; }         // Số lượng
        public decimal UnitPrice { get; set; }        // Đơn giá
        public string Supplier { get; set; }          // Nhà cung cấp
        public string Category { get; set; }          // Nhóm vật tư
        public string Description { get; set; }       // Ghi chú
        public DateTime CreatedDate { get; set; }     // Ngày tạo
        public string Status { get; set; }            // Trạng thái

        public decimal TotalPrice => Quantity * UnitPrice;

        public Material()
        {
            CreatedDate = DateTime.Now;
            Status = "Active";
        }
    }
}