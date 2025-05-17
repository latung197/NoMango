using Astemo.Application.CustomModels.Pagging;

namespace Astemo.Application.CustomModels.SearchConditions
{
    public class FileBackupSearchImpl : PaggingImpl
    {
        public string FileName { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }
}