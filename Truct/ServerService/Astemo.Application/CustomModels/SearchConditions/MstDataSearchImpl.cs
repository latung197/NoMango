using Astemo.Application.CustomModels.Pagging;

namespace Astemo.Application.CustomModels.SearchConditions
{
    public class MstDataSearchImpl : PaggingImpl
    {
        public string Customer { get; set; } = string.Empty;
        public string AssemblyProductCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string HuSerial { get; set; } = string.Empty;
        public string InternalDrawingCode { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
    }
}