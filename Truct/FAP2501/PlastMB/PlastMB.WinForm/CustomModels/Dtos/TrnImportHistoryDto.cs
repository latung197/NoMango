using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.CustomModels.Dtos
{
    public class TrnImportHistoryDto
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string MachineNo { get; set; }

        public int? FactoryCd { get; set; }

        public int? RecordCount { get; set; }

        public string Status { get; set; }

        public string Note { get; set; }

        public DateTime? ImportTime { get; set; }

        public string? Flag { get; set; }
    }
}
