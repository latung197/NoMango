using Cody_v3.Repositories.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Repositories.Entities
{
    public class Department:BaseEntity
    {
        public int Level { get; set; }
        [DataType("varchar")]
        [StringLength(6)]
        [Display(Name = "Mã đơn vị quản lý")]
        public string DepartmentCode { get; set; }
        [DataType("nvarchar")]
        [StringLength(100)]
        [Display(Name = "Tên đơn vị quản lý")]
        public string DepartmentName { get; set; }
        [DataType("nvarchar")]
        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string DepartmentAddress { get; set; }
        [DataType("nvarchar")]
        [StringLength(20)]
        [Display(Name = "Điện thoại")]
        public string PhoneNumber { get; set; }
        [DataType("nvarchar")]
        [StringLength(320)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public virtual IEnumerable<Recloser> Reclosers { get; set; }
    }
}
