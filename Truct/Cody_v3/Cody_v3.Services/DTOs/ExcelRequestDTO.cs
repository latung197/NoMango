using Microsoft.AspNetCore.Http;

namespace Cody_v3.Services.DTOs
{
    public class ExcelRequestDTO
    {
        public IFormFileCollection ExcelFiles { get; set; }
    }
}
