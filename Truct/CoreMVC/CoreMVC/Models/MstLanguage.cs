using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


namespace CoreMVC.Models
{
    [Table("mst_language")]
    public class MstLanguage: BaseEntity
    {
        [Key]
        [Column(TypeName ="varchar(20)")]
        public string item_code {  get; set; }
        [Column(TypeName = "varchar(256)")]
        public string item_name { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string lang_code { get; set; }

    }
}
