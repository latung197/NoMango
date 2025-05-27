using Core.Application.CustomModels.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.CustomModels.Others
{
    //Lấy thông tin in tem đặc biệt
    public class SpecialStampInfo
    {
        public string CustomerCode { get; set; } = string.Empty;
        public string DrawingCode { get; set; } = string.Empty;
        public string AssemblyProductCode { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int ExportType { get; set; }
    }
}
