using Newtonsoft.Json;

namespace PuduIOT.Models
{
    public class Position
    {
        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("yaw")]
        public double Yaw { get; set; }
    }
}
