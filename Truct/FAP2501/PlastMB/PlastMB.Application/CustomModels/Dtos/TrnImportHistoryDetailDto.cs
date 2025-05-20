using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlastMB.Application.CustomModels.Dtos
{
    [Description("TRN_IMPORT_HISTORY_DEAIL")]
    public class TrnImportHistoryDetailDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        [Description("ID tự động tăng")]
        public long ID { get; set; }

        [Column("IDDATA")]
        [Description("ID tham chiếu dữ liệu import")]
        public long? IDData { get; set; }

        [Column("FACTORY_CD")]
        [Description("Mã nhà máy")]
        public int? FactoryCd { get; set; }

        [Column("MACHINE_NO")]
        [MaxLength(5)]
        [Description("Số máy")]
        public string? MachineNo { get; set; }

        [Column("FILENAME")]
        [MaxLength(250)]
        [Description("Tên file")]
        public string? FileName { get; set; }

        [Column("IMPORTTIME")]
        public DateTime? ImportTime { get; set; }
    }
}
