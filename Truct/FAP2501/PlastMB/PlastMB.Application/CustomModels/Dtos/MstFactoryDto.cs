using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlastMB.Application.CustomModels.Dtos
{
    public class MstFactoryDto
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
