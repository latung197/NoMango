using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.Models
{
    [Table("mst_unit")]
    public class MstUnit : DbLayout
    {
        [Key]
        public int id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string unit_code { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string unit_type { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string location { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string item_code { get; set; }

        [Column(TypeName = "numberic(10,2)")]
        public decimal? standard_value { get; set; }
    }
}
