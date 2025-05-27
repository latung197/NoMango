using Core.Application.CustomModels.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.CustomModels.Others
{
    public class ImportPlan
    {
        [Required]
        public string Token {  get; set; }
        [Required]
        public List<ExportListPlanImportDto> Data { get; set; }
    }
}
