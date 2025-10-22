
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Dtos.SystemDtos;

namespace Core.Application.Interface.SysInterface
{
    public interface ISysUserCommandService
    {
        //Task<ServiceResult> Authenticate(Login login);
        //Task<GenericResponseResult<AuthorizedUser>> SearchUser(SysUserSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> InsertUser(SysUserCommandDto dto);
        Task<ServiceResult> UpdateUser(SysUserCommandDto dto);
        Task<ServiceResult> GetUserById(int id);
        Task<ServiceResult> DeleteUser(int id);
        Task<ServiceResult> ChangePassword(ChangePassword dto);
    }
}
