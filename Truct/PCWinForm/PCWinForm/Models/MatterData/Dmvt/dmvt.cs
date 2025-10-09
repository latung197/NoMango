using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCWinForm.Models.MatterData.Dmvt
{
    public class dmvt
    {
        [Key]
        [StringLength(20)]
        public string mavt { get; set; } // Mã vật tư

        [Required]
        [StringLength(255)]
        public string tenvt { get; set; } // Tên vật tư

        [StringLength(20)]
        public string dvt { get; set; } // Đơn vị tính

        public decimal? slton { get; set; } // Số lượng tồn

        public decimal? dongia { get; set; } // Đơn giá

        [StringLength(100)]
        public string nhomvt { get; set; } // Nhóm vật tư

        [StringLength(255)]
        public string nhacc { get; set; } // Nhà cung cấp

        [StringLength(500)]
        public string ghichu { get; set; } // Ghi chú

        public bool? trangthai { get; set; } = true; // Trạng thái

        public DateTime? ngaytao { get; set; } = DateTime.Now; // Ngày tạo

        [StringLength(50)]
        public string nguoitao { get; set; } // Người tạo

        // Tính toán thành tiền
        public decimal? thanhtien => slton * dongia;

        public dmvt()
        {
            ngaytao = DateTime.Now;
            trangthai = true;
        }
    }
}
