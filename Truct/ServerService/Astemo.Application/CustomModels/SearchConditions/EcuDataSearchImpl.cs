using Core.Application.CustomModels.Pagging;

namespace Core.Application.CustomModels.SearchConditions
{
    public class EcuDataSearchImpl : PaggingImpl
    {
        public string HUCode { get; set; } = string.Empty;
        public string LaserPrinting { get; set; } = string.Empty;
        public string? NgCode { get; set; } = string.Empty;
        public int Result { get; set; }
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }
}