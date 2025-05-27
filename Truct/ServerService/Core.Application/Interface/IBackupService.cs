
using Core.Application.CustomModels;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;

namespace Core.Application.Interface
{
    public interface IBackupService
    {
        Task<ServiceResult> Database();
        Task<GenericResponseResult<FileBackupInfo>> SearchFileBackup(FileBackupSearchImpl condition);
    }
}
