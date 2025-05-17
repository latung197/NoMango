using Astemo.Application.CustomModels.Pagging;

namespace Astemo.Application.CustomModels.SearchConditions
{
    public class MstUserSearchImpl //: PaggingImpl
    {
        public string Username { get; set; } = string.Empty;
        public int Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Enable { get; set; }
        public string Fullname { get; set; } = string.Empty;
    }
}