
using PlastMB.Application.CustomModels.Others;
using PlastMB.Application.CustomModels;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels.Dtos;

namespace PlastMB.Application.Interface
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
