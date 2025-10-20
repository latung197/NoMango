using Core.Application.CustomModels.Pagging;

namespace Core.Application.CustomModels.SearchConditions
{
    public class SysUserSearchImpl //: PaggingImpl
    {
        public string Username { get; set; } = string.Empty;
        public int Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Enable { get; set; }
        public string Fullname { get; set; } = string.Empty;
    }
}