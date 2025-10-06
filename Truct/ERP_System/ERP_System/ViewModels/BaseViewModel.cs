using System.Windows.Input;
using ERP_System.Models;
using System.Windows;

namespace ERP_System.ViewModels
{
    public class BaseViewModel : BaseModel
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        protected void ShowMessage(string message, string title = "Thông báo")
        {
            System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        protected void ShowError(string message, string title = "Lỗi")
        {
            System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }

        protected bool ShowConfirmation(string message, string title = "Xác nhận")
        {
            var result = System.Windows.MessageBox.Show(message, title,
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            return result == System.Windows.MessageBoxResult.Yes;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}