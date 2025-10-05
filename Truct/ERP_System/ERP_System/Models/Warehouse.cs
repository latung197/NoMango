namespace ERP_System.Models
{
    public class Warehouse : BaseModel
    {
        private int _warehouseID;
        public int WarehouseID
        {
            get => _warehouseID;
            set => SetProperty(ref _warehouseID, value);
        }

        private string _warehouseCode;
        public string WarehouseCode
        {
            get => _warehouseCode;
            set => SetProperty(ref _warehouseCode, value);
        }

        private string _warehouseName;
        public string WarehouseName
        {
            get => _warehouseName;
            set => SetProperty(ref _warehouseName, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private int _managerID;
        public int ManagerID
        {
            get => _managerID;
            set => SetProperty(ref _managerID, value);
        }

        private string _managerName;
        public string ManagerName
        {
            get => _managerName;
            set => SetProperty(ref _managerName, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}