using Cody_v3.Repositories.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Repositories.Entities
{
    public class Recloser:BaseEntity
    {
        //[DataType("varchar")]
        //[StringLength(6)]
        //[Display(Name ="Mã đơn vị quản lý")]
        //public string RecloserCode { get; set; }

        public Guid DepartmentId { get; set; }

        [DataType("nvarchar")]
        [StringLength(100)]
        [Display(Name = "Tên Recloser")]
        public string RecloserName { get; set; }

        [DataType("nvarchar")]
        [StringLength(500)]
        [Display(Name = "Định danh")]
        public string RecloserQualify { get; set; }

        [Display(Name = "I Định mức")]
        public int I_Norms { get; set; }

        [DataType("nvarchar")]
        [StringLength(100)]
        [Display(Name = "Dây dẫn")]
        public string LineType { get; set; }

        public virtual IEnumerable<RecloserData> RecloserDatas { get; set; }
        public virtual Department Department { get; set; }
    }
}
