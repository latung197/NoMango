using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    public class MstFactorySearchImpl : PaggingImpl
    {
        public string? FACTORY_CD { get; set; } = string.Empty;
        public string? FACTORY_NAME { get; set; } = string.Empty;
    }
}