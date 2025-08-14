using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PuduIOT.Models
{
    [Table("mst_language")]
    public class MstLanguage : DbLayout
    {
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string item_code { get; set; }
        [Column(TypeName = "varchar(240)")]
        public string? display_name { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string lang_code { get; set; }
    }
}
