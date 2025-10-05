namespace ERP_System.Models
{
    public class Material : BaseModel
    {
        private int _materialID;
        public int MaterialID
        {
            get => _materialID;
            set => SetProperty(ref _materialID, value);
        }

        private string _materialCode;
        public string MaterialCode
        {
            get => _materialCode;
            set => SetProperty(ref _materialCode, value);
        }

        private string _materialName;
        public string MaterialName
        {
            get => _materialName;
            set => SetProperty(ref _materialName, value);
        }

        private int _unitID;
        public int UnitID
        {
            get => _unitID;
            set => SetProperty(ref _unitID, value);
        }

        private string _unitName;
        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        private int _materialGroupID;
        public int MaterialGroupID
        {
            get => _materialGroupID;
            set => SetProperty(ref _materialGroupID, value);
        }

        private string _materialGroup;
        public string MaterialGroup
        {
            get => _materialGroup;
            set => SetProperty(ref _materialGroup, value);
        }

        private decimal _minStock;
        public decimal MinStock
        {
            get => _minStock;
            set => SetProperty(ref _minStock, value);
        }

        private decimal _maxStock;
        public decimal MaxStock
        {
            get => _maxStock;
            set => SetProperty(ref _maxStock, value);
        }

        private decimal _currentStock;
        public decimal CurrentStock
        {
            get => _currentStock;
            set => SetProperty(ref _currentStock, value);
        }

        private decimal _costPrice;
        public decimal CostPrice
        {
            get => _costPrice;
            set => SetProperty(ref _costPrice, value);
        }

        private decimal _sellingPrice;
        public decimal SellingPrice
        {
            get => _sellingPrice;
            set => SetProperty(ref _sellingPrice, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private string _stockStatus;
        public string StockStatus
        {
            get => _stockStatus;
            set => SetProperty(ref _stockStatus, value);
        }
    }
}