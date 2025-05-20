using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.CustomModels.SearchConditions
{
    public class TrnImportHistoryDetailSearchImpl
    {
        public string FILENAME {  get; set; } = string.Empty;
        public DateTime IMPORTTIME { get; set; }= DateTime.Now;
    }
}
