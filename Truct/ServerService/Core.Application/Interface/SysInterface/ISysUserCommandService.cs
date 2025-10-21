
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.CustomModels.Dtos;

namespace Core.Application.Interface
{
    public interface ISysUserCommandService
    {
        //Task<ServiceResult> Authenticate(Login login);
        //Task<GenericResponseResult<AuthorizedUser>> SearchUser(SysUserSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> InsertUser(SysUserDto dto);
        Task<ServiceResult> UpdateUser(SysUserDto dto);
        Task<ServiceResult> GetUserById(int id);
        Task<ServiceResult> DeleteUser(int id);
        Task<ServiceResult> ChangePassword(ChangePassword dto);
    }
}
