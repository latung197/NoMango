using System.Collections.ObjectModel;
using System.Windows.Input;
using ERP_System.Models;
using ERP_System.Repositories.Inventory;

namespace ERP_System.ViewModels.Inventory
{
    public class StockRequestViewModel : BaseViewModel
    {
        private readonly StockRequestRepository _stockRequestRepository;
        private readonly MaterialRepository _materialRepository;

        private ObservableCollection<StockRequest> _stockRequests;
        public ObservableCollection<StockRequest> StockRequests
        {
            get => _stockRequests;
            set => SetProperty(ref _stockRequests, value);
        }

        private StockRequest _selectedRequest;
        public StockRequest SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(ref _selectedRequest, value);
                if (value != null)
                {
                    LoadRequestDetails(value.RequestID);
                }
            }
        }

        private ObservableCollection<Material> _availableMaterials;
        public ObservableCollection<Material> AvailableMaterials
        {
            get => _availableMaterials;
            set => SetProperty(ref _availableMaterials, value);
        }

        private StockRequest _currentRequest;
        public StockRequest CurrentRequest
        {
            get => _currentRequest;
            set => SetProperty(ref _currentRequest, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand AddDetailCommand { get; }
        public ICommand RemoveDetailCommand { get; }
        public ICommand RefreshCommand { get; }

        public StockRequestViewModel()
        {
            _stockRequestRepository = new StockRequestRepository();
            _materialRepository = new MaterialRepository();

            StockRequests = new ObservableCollection<StockRequest>();
            AvailableMaterials = new ObservableCollection<Material>();
            CurrentRequest = new StockRequest();

            InitializeCommands();
            LoadData();
            LoadAvailableMaterials();
        }

        private void InitializeCommands()
        {
            LoadDataCommand = new RelayCommand(LoadData);
            NewCommand = new RelayCommand(NewRequest);
            EditCommand = new RelayCommand(EditRequest, CanEditDelete);
            DeleteCommand = new RelayCommand(DeleteRequest, CanEditDelete);
            SaveCommand = new RelayCommand(SaveRequest, CanSave);
            ApproveCommand = new RelayCommand(ApproveRequest, CanApprove);
            AddDetailCommand = new RelayCommand(AddDetail);
            RemoveDetailCommand = new RelayCommand(RemoveDetail);
            RefreshCommand = new RelayCommand(Refresh);
        }

        public void LoadRequest(int requestID)
        {
            try
            {
                var request = _stockRequestRepository.GetStockRequest(requestID);
                if (request != null)
                {
                    CurrentRequest = request;
                    SelectedRequest = request;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải phiếu: {ex.Message}");
            }
        }

        private async void LoadData()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang tải danh sách phiếu yêu cầu...";

                var requests = await Task.Run(() => _stockRequestRepository.GetStockRequests());
                StockRequests = new ObservableCollection<StockRequest>(requests);

                StatusMessage = $"Đã tải {requests.Count} phiếu yêu cầu";
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải danh sách phiếu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadAvailableMaterials()
        {
            try
            {
                var materials = await Task.Run(() => _materialRepository.GetMaterials());
                AvailableMaterials = new ObservableCollection<Material>(
                    materials.Where(m => m.IsActive && m.CurrentStock > 0));
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải danh sách vật tư: {ex.Message}");
            }
        }

        private void LoadRequestDetails(int requestID)
        {
            try
            {
                var request = _stockRequestRepository.GetStockRequest(requestID);
                if (request != null)
                {
                    CurrentRequest = request;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải chi tiết phiếu: {ex.Message}");
            }
        }

        private void NewRequest()
        {
            CurrentRequest = new StockRequest
            {
                RequestCode = _stockRequestRepository.GenerateRequestCode(),
                RequestDate = DateTime.Now,
                Status = "Draft",
                CreatedBy = 1, // TODO: Get from current user
                CreatedByName = "System", // TODO: Get from current user
                Details = new ObservableCollection<StockRequestDetail>()
            };
        }

        private void EditRequest()
        {
            if (SelectedRequest == null) return;
            CurrentRequest = SelectedRequest;
        }

        private void DeleteRequest()
        {
            if (SelectedRequest == null) return;

            if (ShowConfirmation($"Bạn có chắc chắn muốn xóa phiếu {SelectedRequest.RequestCode}?"))
            {
                try
                {
                    if (_stockRequestRepository.DeleteStockRequest(SelectedRequest.RequestID))
                    {
                        StockRequests.Remove(SelectedRequest);
                        ShowMessage("Xóa phiếu thành công");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Lỗi khi xóa phiếu: {ex.Message}");
                }
            }
        }

        private async void SaveRequest()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang lưu phiếu...";

                // Calculate totals
                CurrentRequest.TotalQuantity = CurrentRequest.Details.Sum(d => d.Quantity);
                CurrentRequest.TotalValue = CurrentRequest.Details.Sum(d => d.TotalPrice);

                int requestID;
                if (CurrentRequest.RequestID == 0)
                {
                    requestID = await Task.Run(() => _stockRequestRepository.CreateStockRequest(CurrentRequest));
                    CurrentRequest.RequestID = requestID;
                    StockRequests.Insert(0, CurrentRequest);
                    ShowMessage("Tạo phiếu thành công");
                }
                else
                {
                    var success = await Task.Run(() => _stockRequestRepository.UpdateStockRequest(CurrentRequest));
                    if (success)
                    {
                        // Update the item in the list
                        var existing = StockRequests.FirstOrDefault(r => r.RequestID == CurrentRequest.RequestID);
                        if (existing != null)
                        {
                            var index = StockRequests.IndexOf(existing);
                            StockRequests[index] = CurrentRequest;
                        }
                        ShowMessage("Cập nhật phiếu thành công");
                    }
                }

                StatusMessage = "Lưu phiếu thành công";
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi lưu phiếu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApproveRequest()
        {
            if (SelectedRequest == null) return;

            if (ShowConfirmation($"Bạn có chắc chắn muốn duyệt phiếu {SelectedRequest.RequestCode}?"))
            {
                try
                {
                    if (_stockRequestRepository.ApproveStockRequest(SelectedRequest.RequestID, 1)) // TODO: Get current user
                    {
                        SelectedRequest.Status = "Approved";
                        SelectedRequest.ApprovedBy = 1;
                        SelectedRequest.ApprovedByName = "System";
                        SelectedRequest.ApprovedDate = DateTime.Now;
                        ShowMessage("Duyệt phiếu thành công");

                        // TODO: Create notification
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Lỗi khi duyệt phiếu: {ex.Message}");
                }
            }
        }

        private void AddDetail()
        {
            if (CurrentRequest == null) return;

            var newDetail = new StockRequestDetail
            {
                MaterialID = AvailableMaterials.FirstOrDefault()?.MaterialID ?? 0,
                MaterialCode = AvailableMaterials.FirstOrDefault()?.MaterialCode ?? "",
                MaterialName = AvailableMaterials.FirstOrDefault()?.MaterialName ?? "",
                UnitName = AvailableMaterials.FirstOrDefault()?.UnitName ?? "",
                Quantity = 1,
                UnitPrice = AvailableMaterials.FirstOrDefault()?.CostPrice ?? 0,
                SortOrder = CurrentRequest.Details.Count + 1
            };

            CurrentRequest.Details.Add(newDetail);
        }

        private void RemoveDetail()
        {
            // This would be called from the view with a specific detail
            // For now, just show a message
            ShowMessage("Chức năng xóa dòng");
        }

        private void Refresh()
        {
            LoadData();
            LoadAvailableMaterials();
        }

        private bool CanEditDelete()
        {
            return SelectedRequest != null && SelectedRequest.Status == "Draft";
        }

        private bool CanSave()
        {
            return CurrentRequest != null &&
                   CurrentRequest.Details.Count > 0 &&
                   CurrentRequest.WarehouseID > 0;
        }

        private bool CanApprove()
        {
            return SelectedRequest != null &&
                   SelectedRequest.Status == "Draft" &&
                   SelectedRequest.Details.Count > 0;
        }
    }
}