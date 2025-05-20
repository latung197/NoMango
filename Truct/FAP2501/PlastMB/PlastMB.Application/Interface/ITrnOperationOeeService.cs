
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels;

namespace PlastMB.Application.Interface
{
    public interface ITrnOperationOeeService
    {
        Task<GenericResponseResult<TrnOperationOeeDto>> SearchTrnOperationOee(TrnOperationOeeSearchImpl condition, bool blnExport = false);
        //Task<ServiceResult> UpdateEcuData(EcuDataDto dto);
        //Task<ServiceResult> DeleteEcuData(int id);
        Task<ServiceResult> ImportListTrnOperationOee(List<TrnOperationOeeDto> data);
    }
}
