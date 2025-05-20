using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Domain.Entity
{
    [Description("Bảng MstFactory")]
    [Table(name: "MST_FACTORY", Schema = "public")]
    public class MstFactory
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("ID")]
        public long ID { get; set; }

        [Required]
        [Description("Mã nhà máy")]
        [Column("FACTORY_CD")]
        public int FactoryCd { get; set; }

        [Required]
        [Description("Tên nhà máy")]
        [MaxLength(5)]
        [Column("FACTORY_NAME")]
        public string? FactoryName { get; set; }

    }
}
