using Core.Application.CustomModels;
using Core.Application.CustomModels.Others;

namespace Core.Application.Interface
{
    public interface IHandyService
    {
        Task<ServiceResult> CheckHandyInfo(HandyInfo data);
    }
}
