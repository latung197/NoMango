using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;

namespace Worker.Application.Interface
{
    public interface IWorkerServiceClient
    {
        Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data);
    }
}
