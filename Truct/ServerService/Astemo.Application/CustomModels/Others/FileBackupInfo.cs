
namespace Astemo.Application.CustomModels.Others
{
    /// <summary>
    /// Danh sách các file backup
    /// </summary>
    public class FileBackupInfo
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long FileSize { get; set; }
    }
}
