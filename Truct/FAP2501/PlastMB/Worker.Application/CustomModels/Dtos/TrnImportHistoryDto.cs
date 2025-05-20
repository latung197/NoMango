using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Application.CustomModels.Dtos
{
    public class TrnImportHistoryDto
    {
        [Column("ID")]
        public int ID { get; set; }

        [MaxLength(255)]
        [Column("FILENAME")]

        public string FileName { get; set; }
        [Description("Tên máy")]
        [MaxLength(5)]
        [Column("MACHINE_NO")]
        public string? MachineNo { get; set; }
        [Description("Tên nhà máy")]
        [Column("FACTORY_CD")]
        public int? FactoryCd { get; set; }

        [Description("Tổng số dòng Import")]
        [Column("RECORDCOUNT")]
        public int? RecordCount { get; set; }
        [Description("Trạng thái import")]
        [MaxLength(200)]
        [Column("STATUS")]
        public string Status { get; set; }

        [Description("Ghi chú")]
        [MaxLength(500)]
        [Column("NOTE")]
        public string Note { get; set; }

        [Column("IMPORTTIME")]
        public DateTime? ImportTime { get; set; }

        [MaxLength(1)]
        [Column("FLAG")]
        public string? Flag { get; set; }
    }
}
