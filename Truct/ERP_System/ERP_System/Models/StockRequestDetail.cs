namespace ERP_System.Models
{
    public class StockRequestDetail : BaseModel
    {
        private int _detailID;
        public int DetailID
        {
            get => _detailID;
            set => SetProperty(ref _detailID, value);
        }

        private int _requestID;
        public int RequestID
        {
            get => _requestID;
            set => SetProperty(ref _requestID, value);
        }

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

        private string _unitName;
        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        private decimal _quantity;
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                CalculateTotalPrice();
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                SetProperty(ref _unitPrice, value);
                CalculateTotalPrice();
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        private string _note;
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        private int _sortOrder;
        public int SortOrder
        {
            get => _sortOrder;
            set => SetProperty(ref _sortOrder, value);
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = Quantity * UnitPrice;
        }
    }
}