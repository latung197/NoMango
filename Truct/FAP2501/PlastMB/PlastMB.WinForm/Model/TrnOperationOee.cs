using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Model
{
    public class TrnOperationOee
    {


        public DateTime? AchievementRegistrationDate { get; set; }

        public DateTime? AchievementRegistrationTime { get; set; }

        public string? ProcessingTime { get; set; }

        public string? ProcessingStopTime { get; set; }

        public string? LossStopTime { get; set; }

        public int? ProductionCount { get; set; }

        public decimal? OperationRate { get; set; }

        public string? EquipmentOperationHours { get; set; }

        public string? LoadTime { get; set; }

        public decimal? TimeOperatingRate { get; set; }
    }
}
