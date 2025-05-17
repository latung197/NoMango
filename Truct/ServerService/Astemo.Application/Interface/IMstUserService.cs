
using Astemo.Application.CustomModels.Others;
using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.SearchConditions;
using Astemo.Application.CustomModels.Dtos;

namespace Astemo.Application.Interface
{
    public interface IMstUserService
    {
        Task<ServiceResult> Authenticate(Login login);
        Task<GenericResponseResult<AuthorizedUser>> SearchUser(MstUserSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> InsertUser(MstUserDto dto);
        Task<ServiceResult> UpdateUser(MstUserDto dto);
        Task<ServiceResult> GetUserById(int id);
        Task<ServiceResult> DeleteUser(int id);
        Task<ServiceResult> ChangePassword(ChangePassword dto);
    }
}
