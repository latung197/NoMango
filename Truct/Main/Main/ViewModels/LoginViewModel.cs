using Main.Models;
using Main.Repositories;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Security.Principal;

namespace Main.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _userName;
        private SecureString _passWord;
        private string _errorMessage;
        private bool _isViewVisible = true;

        private IUserRepository userRepository;
        public string Username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public SecureString Password
        {
            get { return _passWord; }
            set
            {
                _passWord = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ErrorMessage
        {
            get
            { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteloginCommand, CanExecuteloginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExcuteRecoverPassCommand("", ""));
        }

        private void ExcuteRecoverPassCommand(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private bool CanExecuteloginCommand(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 || Password == null || Password.Length < 3)
            {
                validData = false;
            }
            else
            {
                validData = true;
            }
            return validData;
        }

        private void ExecuteloginCommand(object obj)
        {
            var isValuidUser = userRepository.AuthenticationUser(new NetworkCredential(Username, Password));
            if (isValuidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(Username), null
                    );
                isValuidUser = false;
            }
            else
            {
                ErrorMessage = "* Invalid username or password";
            }
        }
    }
}
