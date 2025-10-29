using Cody_v3.Repositories.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Repositories.Entities
{
    public class RecloserData : BaseEntity
    {
        [DataType("nvarchar")]
        [StringLength(100)]
        public string SheetName { get; set; }
        
        [DataType("nvarchar")]
        [StringLength(1024)]
        public string ExcelFileName { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }
        
        [DataType("varchar")]
        [StringLength(20)]
        public string Time { get; set; }
        //[StringLength(500)]
        //[DataType("nvarchar")]
        //public string Recloser { get; set; }
        public Guid RecloserId { get; set; }
        public decimal? U_kV { get; set; }
        public decimal? I_A { get; set; }
        public decimal? P_MW { get; set; }
        public decimal? Q_MVar { get; set; }
        public decimal? COS_φ { get; set; }

        public virtual Recloser Recloser { get; set; }
    }
}
