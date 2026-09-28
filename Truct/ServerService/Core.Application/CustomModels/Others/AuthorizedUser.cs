
namespace Core.Application.CustomModels.Others
{
    public class AuthorizedUser
    {
        public int UserId {  get; set; }
        public string Username {  get; set; }
        public string Fullname { get; set; }
        public string Employeecode { get; set; }
        public string Email {  get; set; }
        public List<int> Role {  get; set; }
        public string Token {  get; set; }
        public List<Core.Application.Security.PermissionGrantDto> Permissions { get; set; } = [];
        public bool IsAdmin { get; set; }
        public List<int> GroupRoleIds { get; set; } = [];
    }
}
