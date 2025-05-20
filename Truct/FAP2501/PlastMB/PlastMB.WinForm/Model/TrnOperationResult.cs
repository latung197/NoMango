using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Model
{
 
    public class TrnOperationResult
    {
        public DateTime? AchievementRegistrationDate { get; set; }
        public DateTime? AchievementRegistrationTime { get; set; }
        public string MachineNo { get; set; }
        public DateTime? ProcessingStartTime { get; set; }
        public DateTime? ProcessingEndTime { get; set; }
        public string? ProcessingTime { get; set; }
        public string? ProgressRate { get; set; }
        public string? StandardTime { get; set; }
        public string? ProcessingStopTime { get; set; }
        public string? LossStopTime { get; set; }
        public string? SlipNo { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ProductName1 { get; set; }
        public string? ProductName2 { get; set; }
        public int? Value { get; set; }
        public string? Operator1 { get; set; }
        public string? Operator2 { get; set; }
        public string? Operator3 { get; set; }
        public int? MeasurementInspection { get; set; }
        public int? Changeover { get; set; }
        public int? CAD { get; set; }
        public int? EquipmentFailure { get; set; }
        public int? Cleaning { get; set; }
        public int? RestTime { get; set; }
    }
}
