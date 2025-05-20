
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels;

namespace PlastMB.Application.Interface
{
    public interface ITrnOperationResultService
    {
        Task<GenericResponseResult<TrnOperationResultDto>> SearchTrnOperationResult(TrnOperationResultSearchImpl condition, bool blnExport = false);
        //Task<ServiceResult> UpdateEcuData(EcuDataDto dto);
        //Task<ServiceResult> DeleteEcuData(int id);
        Task<ServiceResult> ImportListTrnOperationResult(List<TrnOperationResultDto> data);
    }
}
