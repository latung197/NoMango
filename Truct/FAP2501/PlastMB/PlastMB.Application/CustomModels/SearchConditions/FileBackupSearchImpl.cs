using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    public class FileBackupSearchImpl : PaggingImpl
    {
        public string FileName { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }
}