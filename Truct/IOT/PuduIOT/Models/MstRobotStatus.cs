using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.Models
{
    [Table("mst_robot_status")]
    public class MstRobotStatus
    {
        [Key]
        [Column("id", TypeName = "bigint")]
        public long Id { get; set; }

        [Column("sn", TypeName = "varchar(125)")]
        public string Sn { get; set; }

        [Column("battery", TypeName = "integer")]
        public int? Battery { get; set; }

        [Column("charge_stage", TypeName = "varchar(125)")]
        public string ChargeStage { get; set; }

        [Column("device_name", TypeName = "varchar(125)")]
        public string DeviceName { get; set; }

        [Column("is_charging", TypeName = "integer")]
        public int? IsCharging { get; set; }

        [Column("is_online", TypeName = "integer")]
        public int? IsOnline { get; set; }

        [Column("map_name", TypeName = "integer")]
        public int? MapName { get; set; }

        [Column("move_state", TypeName = "integer")]
        public int? MoveState { get; set; }

        [Column("remain_time", TypeName = "integer")]
        public int? RemainTime { get; set; }

        [Column("schedule_msg", TypeName = "varchar(125)")]
        public string ScheduleMsg { get; set; }

        [Column("schedule_status", TypeName = "interval")]
        public TimeSpan? ScheduleStatus { get; set; }

        [Column("timestamp", TypeName = "varchar(125)")]
        public string Timestamp { get; set; }

        [Column("work_msg", TypeName = "varchar(125)")]
        public string WorkMsg { get; set; }

        [Column("work_status", TypeName = "interval")]
        public TimeSpan? WorkStatus { get; set; }
    }
}
