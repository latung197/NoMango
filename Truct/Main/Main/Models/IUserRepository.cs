using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Main.Models
{
    public interface IUserRepository
    {
        bool AuthenticationUser(NetworkCredential credential);
        void Add (UserModel user);
        void Edit(UserModel user);
        void Remove(int id);
        UserModel GetById(int id);
        UserModel GetUserName(string username);
        IEnumerable<UserModel> GetAll();    
    }
}
