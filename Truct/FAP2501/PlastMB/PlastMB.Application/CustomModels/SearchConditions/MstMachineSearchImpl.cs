using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    public class MstMachineSearchImpl : PaggingImpl
    {
        public string? HMI_NO { get; set; } = string.Empty;
        public string? MACHINE_NO { get; set; } = string.Empty;
        public string? MACHINE_NAME { get; set; } = string.Empty;
    }
}