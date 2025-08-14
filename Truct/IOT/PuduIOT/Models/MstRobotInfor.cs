using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.Models
{
    [Table("mst_robot_infor")]
    public class MstRobotInfor
    {
        [Key]
        public int id { get; set; }
        [Column(TypeName = "varchar(125)")]
        public string name { get; set; }
        [Column(TypeName = "varchar(125)")]
        public string status { get; set; }
        [Column(TypeName = "varchar(125)")]
        public string location { get; set; }
        [Column(TypeName = "float")]
        public double battery { get; set; }
        [Column(TypeName = "varchar(125)")]
        public string statusbatteryname {  get; set; }
    }
}
