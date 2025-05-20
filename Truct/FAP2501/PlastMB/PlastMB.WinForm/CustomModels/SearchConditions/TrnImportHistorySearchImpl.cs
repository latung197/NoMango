using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.CustomModels.SearchConditions
{
    public class TrnImportHistorySearchImpl
    {
        public string FileName { get; set; } = string.Empty;
        public string MachineNo { get; set; } = string.Empty;
        public string FactoryCd { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
