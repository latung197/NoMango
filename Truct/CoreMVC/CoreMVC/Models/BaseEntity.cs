using System.ComponentModel.DataAnnotations.Schema;

namespace CoreMVC.Models
{
    public class BaseEntity
    {
        [Column(TypeName = "timestamp")]
        public DateTime? create_time { get; set; }
        [Column(TypeName = "varchar(5)")]
        public string? create_id { get; set; }
        [Column(TypeName = "timestamp ")]
        public DateTime? update_time { get; set; }
        [Column(TypeName = "varchar(5)")]
        public string? update_id { get; set; }
    }
}
