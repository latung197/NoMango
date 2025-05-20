using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Application.CustomModels.SearchConditions
{
    public class MstMachineSearchImpl
    {
        public string? HMI_NO { get; set; } = string.Empty;
        public string? MACHINE_NO { get; set; } = string.Empty;
        public string? MACHINE_NAME { get; set; } = string.Empty;
    }
}
