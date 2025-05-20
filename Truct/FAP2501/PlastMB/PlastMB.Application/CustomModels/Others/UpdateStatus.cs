
using System.ComponentModel;

namespace PlastMB.Application.CustomModels.Others
{
    /// <summary>
    /// Update các entity với trạng thái được chỉ định dạng int
    /// </summary>
    public class UpdateStatus
    {
        public int ID { get; set; }
        public int BoxID { get; set; }
        public string BoxSerial { get; set; }
        public int Status { get; set; }
    }
}