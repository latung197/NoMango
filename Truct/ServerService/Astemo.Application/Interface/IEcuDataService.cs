
using Astemo.Application.CustomModels.Dtos;
using Astemo.Application.CustomModels.SearchConditions;
using Astemo.Application.CustomModels;

namespace Astemo.Application.Interface
{
    public interface IEcuDataService
    {
        Task<GenericResponseResult<EcuDataDto>> SearchEcuData(EcuDataSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> UpdateEcuData(EcuDataDto dto);
        Task<ServiceResult> DeleteEcuData(int id);
        Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data);
    }
}
