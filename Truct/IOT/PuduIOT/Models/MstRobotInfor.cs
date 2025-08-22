using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.Models
{
    [Table("mst_robot_infor")]
    public class MstRobotInfor
    {
        [Key]
        [Column("id", TypeName = "bigint")]
        public int Id { get; set; }
        [Column("sn", TypeName = "varchar(125)")]
        public string Sn {  get; set; }
        [Column("name", TypeName = "varchar(125)")]
        public string Name {  get; set; }
        [Column("company_id", TypeName = "varchar(125)")]
        public string CompanyId {  get; set; }
        [Column("company_name", TypeName = "varchar(125)")]
        public string CompanyName { get; set; }
        [Column("img_name", TypeName = "varchar(125)")]
        public string ImgName { get; set; }
    }
}
