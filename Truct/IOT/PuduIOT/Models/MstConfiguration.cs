using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.Models
{
   
    [Table("mst_configuration")]
    public class MstConfiguration : DbLayout
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("key",TypeName = "varchar(50)")]
        public string? Key { get; set; }
        [Column("value",TypeName = "varchar(50)")]
        public string Value { get; set; }
        [Column("description",TypeName = "varchar(500)")]
        public string Description { get; set; }
    }
}
