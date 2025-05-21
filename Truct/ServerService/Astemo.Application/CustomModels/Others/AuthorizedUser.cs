
namespace Astemo.Application.CustomModels.Others
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
    }
}