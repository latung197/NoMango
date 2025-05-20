using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    public class TrnImportHistoryDetailSearchImpl : PaggingImpl
    {
        public int? IDDATA { get; set; } 
        public string FileName { get; set; } = string.Empty;
        public string MachineNo { get; set; } = string.Empty;
        public string FactoryCd { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }
}