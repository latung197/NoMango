using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Domain.Entity
{
    [Description("Bảng MstMachine")]
    [Table(name: "MST_MACHINE", Schema = "public")]
    public class MstMachine
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("ID")]
        public long ID { get; set; }

        [Required]
        [Description("Mã HMI")]
        [MaxLength(5)]
        [Column("HMI_NO")]
        public string? HmiNo { get; set; }

        [Required]
        [Description("Mã MÁY")]
        [MaxLength(5)]
        [Column("MACHINE_NO")]
        public string? MachineNo { get; set; }

        [Description("Tên MÁY")]
        [MaxLength(10)]
        [Column("MACHINE_NAME")]
        public string? MachineName { get; set; }

        [Description("Vị trí")]
        [Column("INSTALLATION_LOCATION_CD")]
        public int? InstallationLocationCd { get; set; }


    }
}
