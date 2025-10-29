using Cody_v3.Repositories.Entities;
using Cody_v3.Repositories.Paging;
using Cody_v3.Services.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.Interfaces
{
    public interface IRecloserDataService : IGenericService<RecloserData>
    {
        Task<List<RecloserData>> GetAllCurrent();

        Task<bool> Uploads(IFormFileCollection excelFiles, string folderSave, bool generateFileName = false);

        Task<int> ImportData(IFormFileCollection excelFiles, string folderSave);

        Task<PageResult<RecloserDataResponseDTO>> GetWithPaging(int currentPage, int pageSize, string actionName);
        Task<List<RecloserDataResponseDTO>> GetRecloserDataResponseDTOs(ReportRequestDTO dto);

        Task<List<OverloadReportResponseDTO>> GetOverloadReportResponseDTOs(ReportRequestDTO dto);

        Task<string> UploadImage(IFormFile image);
    }
}
