using Astemo.Application.CustomModels.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Astemo.Application.CustomModels.Others
{
    //Thông tin gửi từ máy Handy để check Ok/NG
    public class HandyInfo
    {
        [Required]
        public string HUCode { get; set; } = string.Empty;
        [Required]
        public string ProductCode { get; set; } = string.Empty;
        [Required]
        public string CustomerCode { get; set; } = string.Empty;
        public int Type { get; set; }//EnumMasterType
    }
}
