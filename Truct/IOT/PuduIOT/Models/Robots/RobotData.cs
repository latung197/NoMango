using Newtonsoft.Json;

namespace PuduIOT.Models.RobotsT300
{
    public class RobotData
    {
        [JsonProperty("sn")]
        public string Sn { get; set; }
        [JsonProperty("name")]
        public string Name {  get; set; }
        [JsonProperty("company_id")]
        public string Company_id { get; set; }
        [JsonProperty("battery")]
        public int Battery { get; set; }

        [JsonProperty("charge_stage")]
        public string ChargeStage { get; set; }

        [JsonProperty("device_name")]
        public string DeviceName { get; set; }

        [JsonProperty("is_charging")]
        public int IsCharging { get; set; }

        [JsonProperty("is_online")]
        public int IsOnline { get; set; }

        [JsonProperty("map_name")]
        public string MapName { get; set; }

        [JsonProperty("move_state")]
        public string MoveState { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("remain_time")]
        public int RemainTime { get; set; }

        [JsonProperty("schedule_msg")]
        public string ScheduleMsg { get; set; }

        [JsonProperty("schedule_status")]
        public int ScheduleStatus { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("work_msg")]
        public string WorkMsg { get; set; }

        [JsonProperty("work_status")]
        public int WorkStatus { get; set; }
        [JsonProperty("img_name")]
        public string ImagName { get; set; }
    }
}
