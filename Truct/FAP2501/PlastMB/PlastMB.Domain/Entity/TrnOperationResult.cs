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
    [Description("Bảng TRN_OPERATION_RESULTS ")]
    [Table("TRN_OPERATION_RESULTS", Schema = "public")]
    public class TrnOperationResult
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("FACTORY_CD")]
        public int FactoryCd { get; set; }
        [Column("MACHINE_NO")]
        [MaxLength(5)]
        public string MachineNo { get; set; }

        [Column("SHIFT_ID")]
        public int? ShiftId { get; set; }

        [Column("LINE_ID")]
        public int? LineId { get; set; }

        [Column("PROCESS_ID")]
        public int? ProcessId { get; set; }

        [Column("ACHIEVEMENT_REGISTRATION_DATE", TypeName = "date")]
        public DateTime? AchievementRegistrationDate { get; set; }

        [Column("ACHIEVEMENT_REGISTRATION_TIME")]
        public DateTime? AchievementRegistrationTime { get; set; }

        [Column("SLIP_NO")]
        [MaxLength(10)]
        public string? SlipNo { get; set; }

        [Column("CUSTOMER_CD")]
        [MaxLength(10)]
        public string? CustomerCd { get; set; }

        [Column("CUSTOMER_NAME")]
        [MaxLength(50)]
        public string? CustomerName { get; set; }

        [Column("DUE_DATE", TypeName = "date")]
        public DateTime? DueDate { get; set; }

        [Column("PRODUCT_NAME_1")]
        [MaxLength(50)]
        public string? ProductName1 { get; set; }

        [Column("PRODUCT_NAME_2")]
        [MaxLength(50)]
        public string? ProductName2 { get; set; }

        [Column("VALUE")]
        public int? Value { get; set; }

        [Column("OPERATOR_1")]
        [MaxLength(50)]
        public string? Operator1 { get; set; }

        [Column("OPERATOR_2")]
        [MaxLength(50)]
        public string? Operator2 { get; set; }

        [Column("OPERATOR_3")]
        [MaxLength(50)]
        public string? Operator3 { get; set; }

        [Column("PROCESSING_START_TIME")]
        public DateTime? ProcessingStartTime { get; set; }

        [Column("PROCESSING_END_TIME")]
        public DateTime? ProcessingEndTime { get; set; }

        [Column("PROCESSING_TIME")]
        public int? ProcessingTime { get; set; }

        [Column("PROGRESS_RATE")]
        [MaxLength(5)]
        public string? ProgressRate { get; set; }

        [Column("STANDARD_TIME")]
        public int? StandardTime { get; set; }

        [Column("PROCESSING_STOP_TIME")]
        public int? ProcessingStopTime { get; set; }

        [Column("LOSS_STOP_TIME")]
        public int? LossStopTime { get; set; }

        [Column("MEASUREMENT_INSPECTION")]
        public int? MeasurementInspection { get; set; }

        [Column("CHANGEOVER")]
        public int? Changeover { get; set; }

        [Column("CAD")]
        public int? CAD { get; set; }

        [Column("EQUIPMENT_FAILURE")]
        public int? EquipmentFailure { get; set; }

        [Column("CLEANING")]
        public int? Cleaning { get; set; }
        [Column("REST_TIME")]
        public int? RestTime { get; set; }

    }
}
