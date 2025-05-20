
using Worker.Application.CustomModels;

namespace Worker.Application.Interface
{
    public interface IWorkerService
    {
        Task ScanFolder( string path);
        Task ScanFolder(FolderPaths folderPaths);
        void Stop();

    }
}
