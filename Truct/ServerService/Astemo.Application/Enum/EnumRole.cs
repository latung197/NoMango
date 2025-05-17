using System.ComponentModel;

namespace Astemo.Application.Enum
{
    /// <summary>
    /// Phân quyền cho user
    /// </summary>
    public static class EnumRole
    {
        public enum Role
        {
            [Description("Admin")]
            Admin = 0,
            [Description("Xuất kho")]
            Exported = 1, 
            [Description("Tạo kế hoạch xuất kho")]
            CreateExportPlan = 2
        }
    }
}
