using System;
using System.Collections.ObjectModel;

namespace ERP_System.Models
{
    public class StockRequest : BaseModel
    {
        private int _requestID;
        public int RequestID
        {
            get => _requestID;
            set => SetProperty(ref _requestID, value);
        }

        private string _requestCode;
        public string RequestCode
        {
            get => _requestCode;
            set => SetProperty(ref _requestCode, value);
        }

        private DateTime _requestDate;
        public DateTime RequestDate
        {
            get => _requestDate;
            set => SetProperty(ref _requestDate, value);
        }

        private int _warehouseID;
        public int WarehouseID
        {
            get => _warehouseID;
            set => SetProperty(ref _warehouseID, value);
        }

        private string _warehouseName;
        public string WarehouseName
        {
            get => _warehouseName;
            set => SetProperty(ref _warehouseName, value);
        }

        private string _department;
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private decimal _totalQuantity;
        public decimal TotalQuantity
        {
            get => _totalQuantity;
            set => SetProperty(ref _totalQuantity, value);
        }

        private decimal _totalValue;
        public decimal TotalValue
        {
            get => _totalValue;
            set => SetProperty(ref _totalValue, value);
        }

        private int _createdBy;
        public int CreatedBy
        {
            get => _createdBy;
            set => SetProperty(ref _createdBy, value);
        }

        private string _createdByName;
        public string CreatedByName
        {
            get => _createdByName;
            set => SetProperty(ref _createdByName, value);
        }

        private int _approvedBy;
        public int ApprovedBy
        {
            get => _approvedBy;
            set => SetProperty(ref _approvedBy, value);
        }

        private string _approvedByName;
        public string ApprovedByName
        {
            get => _approvedByName;
            set => SetProperty(ref _approvedByName, value);
        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        private DateTime? _approvedDate;
        public DateTime? ApprovedDate
        {
            get => _approvedDate;
            set => SetProperty(ref _approvedDate, value);
        }

        private ObservableCollection<StockRequestDetail> _details;
        public ObservableCollection<StockRequestDetail> Details
        {
            get => _details ??= new ObservableCollection<StockRequestDetail>();
            set => SetProperty(ref _details, value);
        }
    }
}