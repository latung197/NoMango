using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.CustomModels.SearchConditions;

namespace Worker.Application.Interface
{
    public interface IWorkerServiceClient
    {
        Task<ServiceResult> ImportListTrnOperationOee(List<TrnOperationOeeDto> data);
        Task<ServiceResult> ImportListTrnOperationResult(List<TrnOperationResultDto> data);
        Task<ServiceResult> ImportListTrnImportHistory(List<TrnImportHistoryDto> data);
        Task<string> SearchMstMachine(string data);

    }
}
