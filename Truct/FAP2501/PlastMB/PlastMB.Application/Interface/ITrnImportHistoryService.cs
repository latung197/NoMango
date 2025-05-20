
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels;

namespace PlastMB.Application.Interface
{
    public interface ITrnImportHistoryService
    {
        Task<GenericResponseResult<TrnImportHistoryDto>> SearchImportHistory(TrnImportHistorySearchImpl condition, bool blnExport = false);
        Task<ServiceResult> ImportListImportHistory(List<TrnImportHistoryDto> data);
        
    }
}
