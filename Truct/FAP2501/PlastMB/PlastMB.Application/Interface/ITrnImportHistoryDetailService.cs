
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels;

namespace PlastMB.Application.Interface
{
    public interface ITrnImportHistoryDetailService
    {
        Task<GenericResponseResult<TrnImportHistoryDetailDto>> SearchImportHistoryDetail(TrnImportHistoryDetailSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> ImportListImportHistoryDetail(List<TrnImportHistoryDetailDto> data);
    }
}
