using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PlastMB.Domain.Abstractions;

namespace PlastMB.Domain.Entity
{
    [Description("Bảng CSV lịch sử Import")]
    [Table(name: "TRN_IMPORT_HISTORY_DETAIL", Schema = "public")]
    public class TrnImportHistoryDetail 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        [Description("ID tự động tăng")]
        public long ID { get; set; }

        [Column("IDDATA")]
        [Description("ID tham chiếu dữ liệu import")]
        public long IDData { get; set; }

        [Column("FACTORY_CD")]
        [Description("Mã nhà máy")]
        public int FactoryCd { get; set; }

        [Column("MACHINE_NO")]
        [MaxLength(5)]
        [Description("Số máy")]
        public string MachineNo { get; set; }

        [Column("FILENAME")]
        [MaxLength(250)]
        [Description("Tên file")]
        public string FileName { get; set; }

        [Column("IMPORTTIME")]
        public DateTime ImportTime { get; set; }
    }
}
