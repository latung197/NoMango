using Main.Models;
using Main.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private UserAccountModel _curentUserAccount;
        private IUserRepository userRepository;
        public UserAccountModel CurentUserAccount
        {
            get { return _curentUserAccount; }
            set
            {
                _curentUserAccount = value;
                OnPropertyChanged(nameof(CurentUserAccount));
            }
        }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurentUserAccount = new UserAccountModel();

        }

        private void LoadCurrentUserData()
        {
            var user = userRepository.GetUserName(Thread.CurrentPrincipal.Identity.Name);
            if(user != null)
            {
                CurentUserAccount.Username = user.Username;
                CurentUserAccount.DisplayName = $"Xin chào{user.Name} {user.LastName};";
                CurentUserAccount.ProfilePicture = null;
            }
            else
            {
                CurentUserAccount.DisplayName = "Invalid user, not logged in";
            }
        }
    }
}
