using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Abstractions;
using Core.Domain.Entity.SystemEntities;

namespace Core.Application.CustomModels.Dtos.SystemDtos
{
    public class SysUserCommandDto
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("menuid0")]
        [StringLength(20)]
        public string MenuId0 { get; set; } = string.Empty;

        [Column("can_view")]
        public bool CanView { get; set; } = false;

        [Column("can_add")]
        public bool CanAdd { get; set; } = false;

        [Column("can_edit")]
        public bool CanEdit { get; set; } = false;

        [Column("can_delete")]
        public bool CanDelete { get; set; } = false;

        [Column("can_print")]
        public bool CanPrint { get; set; } = false;

        [Column("can_import")]
        public bool CanImport { get; set; } = false;

        [Column("can_export")]
        public bool CanExport { get; set; } = false;

        [Column("can_search")]
        public bool CanSearch { get; set; } = false;

        [Column("can_reload")]
        public bool CanReload { get; set; } = false;

        [Column("can_copy")]
        public bool CanCopy { get; set; } = false;

        [Column("can_approve")]
        public bool CanApprove { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual SysUser User { get; set; }

        [ForeignKey("MenuId0")]
        public virtual SysCommand Command { get; set; }

    }
}
