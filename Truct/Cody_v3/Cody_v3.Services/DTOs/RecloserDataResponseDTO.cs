using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.DTOs
{
    public class RecloserDataResponseDTO
    {
        [Display(Name = "Excel file")]
        public string ExcelFileName { get; set; }
        [Display(Name ="Sheet")]
        public string SheetName { get; set; }
        [Display(Name = "Bộ phận quản lý")]
        public string DepartmentName { get; set; }
        [Display(Name="Ngày")]
        public DateTime? Date { get; set; }
        [Display(Name ="Giờ")]
        public string Time { get; set; }
        [Display(Name = "Tên Recloser")]
        public string RecloserQualify { get; set; }
        [Display(Name = "U (kV)")]
        public decimal? U_kV { get; set; }
        [Display(Name = "I (A)")]
        public decimal? I_A { get; set; }
        [Display(Name = "P (MW)")]
        public decimal? P_MW { get; set; }
        [Display(Name = "Q (MVar)")]
        public decimal? Q_MVar { get; set; }
        [Display(Name = "COSφ")]
        public decimal? COS_φ { get; set; }
    }
}
