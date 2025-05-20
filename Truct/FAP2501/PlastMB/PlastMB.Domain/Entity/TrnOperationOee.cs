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
    [Description("Bảng TRN_OPERATION_OEE ")]
    [Table(name: "TRN_OPERATION_OEE", Schema = "public")]
    public class TrnOperationOee
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long Id { get; set; }

        [Column("FACTORY_CD")]
        public int FactoryCd { get; set; }

        [Column("SHIFT_ID")]
        public int? ShiftId { get; set; }

        [Column("LINE_ID")]
        public int? LineId { get; set; }

        [Column("PROCESS_ID")]
        public int? ProcessId { get; set; }
        
        [Column("MACHINE_NO")]
        [MaxLength(5)]
        public string MachineNo { get; set; }

        [Column("ACHIEVEMENT_REGISTRATION_DATE", TypeName = "date")]
        public DateTime? AchievementRegistrationDate { get; set; }

        [Column("ACHIEVEMENT_REGISTRATION_TIME")]
        public DateTime? AchievementRegistrationTime { get; set; }

        [Column("PROCESSING_TIME")]
        public int? ProcessingTime { get; set; }

        [Column("PROCESSING_STOP_TIME")]
        public int? ProcessingStopTime { get; set; }

        [Column("LOSS_STOP_TIME")]
        public int? LossStopTime { get; set; }

        [Column("PRODUCTION_COUNT")]
        public int? ProductionCount { get; set; }

        [Column("OPERATION_RATE")]
        public decimal? OperationRate { get; set; }

        [Column("EQUIPMENT_OPERATION_HOURS")]
        public int? EquipmentOperationHours { get; set; }

        [Column("LOAD_TIME")]
        public int? LoadTime { get; set; }

        [Column("TIME_OPERATING_RATE")]
        public decimal? TimeOperatingRate { get; set; }

    }
}
