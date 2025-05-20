using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    public class TrnOperationResultSearchImpl : PaggingImpl
    {
        public string? FILENAME { get; set; } = string.Empty;
        public DateTime? IMPORTTIME { get; set; }
    }
}