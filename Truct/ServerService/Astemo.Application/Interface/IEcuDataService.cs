
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.CustomModels;

namespace Core.Application.Interface
{
    public interface IEcuDataService
    {
        Task<GenericResponseResult<EcuDataDto>> SearchEcuData(EcuDataSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> UpdateEcuData(EcuDataDto dto);
        Task<ServiceResult> DeleteEcuData(int id);
        Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data);
    }
}
