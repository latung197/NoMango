namespace ERP_System.Models
{
    public class DatabaseConnection : BaseModel
    {
        private int _connectionID;
        public int ConnectionID
        {
            get => _connectionID;
            set => SetProperty(ref _connectionID, value);
        }

        private string _connectionName;
        public string ConnectionName
        {
            get => _connectionName;
            set => SetProperty(ref _connectionName, value);
        }

        private string _serverName;
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }

        private string _databaseName;
        public string DatabaseName
        {
            get => _databaseName;
            set => SetProperty(ref _databaseName, value);
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private int _companyID;
        public int CompanyID
        {
            get => _companyID;
            set => SetProperty(ref _companyID, value);
        }

        private string _companyName;
        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}