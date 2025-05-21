using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.Others;

namespace Astemo.Application.Interface
{
    public interface IHandyService
    {
        Task<ServiceResult> CheckHandyInfo(HandyInfo data);
    }
}
