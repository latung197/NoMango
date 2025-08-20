using Newtonsoft.Json;
using PuduIOT.Models.RobotsT300;

namespace PuduIOT.Models
{
    public class RobotResponse
    {
        [JsonProperty("data")]
        public RobotData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("trace_id")]
        public string TraceId { get; set; }
    }
}
