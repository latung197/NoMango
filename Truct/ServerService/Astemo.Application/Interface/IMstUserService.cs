
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.CustomModels.Dtos;

namespace Core.Application.Interface
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
