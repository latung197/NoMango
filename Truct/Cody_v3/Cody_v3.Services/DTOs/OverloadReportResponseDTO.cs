using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.DTOs
{
    public class OverloadReportResponseDTO
    {
        [Display(Name ="TT")]
        public int Row_ID { get; set; }
        [Display(Name ="Tên Recloser")]
        public string RecloserQualify { get; set; }
        [Display(Name = "Idm\n(A)")]
        public int I_Norms { get; set; }
        [Display(Name = "Imax\n(A)")]
        public double Max_I_A { get; set; }
        [Display(Name = "Itb\n(A)")]
        public double AVG_I_A { get; set; }
        [Display(Name = "Imax/Idm\n(%)")]
        public double Percent_I_A { get; set; }
        [Display(Name = "Số lần quá tải")]
        public int Count_OverLoad { get; set; }
        [Display(Name = "Tổng thời gian\nquá tải\n(phút)")]
        public int Minutes_OverLoad { get; set; }
        [Display(Name = "Thời gian")]
        public string Max_Time { get; set; }
        [Display(Name = "Ngày")]
        public DateTime Date { get; set; }   

    }
}
