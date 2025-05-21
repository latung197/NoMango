
using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.Others;
using Astemo.Application.CustomModels.SearchConditions;

namespace Astemo.Application.Interface
{
    public interface IBackupService
    {
        Task<ServiceResult> Database();
        Task<GenericResponseResult<FileBackupInfo>> SearchFileBackup(FileBackupSearchImpl condition);
    }
}
