
using PlastMB.Application.CustomModels;
using PlastMB.Application.CustomModels.Others;
using PlastMB.Application.CustomModels.SearchConditions;

namespace PlastMB.Application.Interface
{
    public interface IBackupService
    {
        Task<ServiceResult> Database();
        Task<GenericResponseResult<FileBackupInfo>> SearchFileBackup(FileBackupSearchImpl condition);
    }
}
