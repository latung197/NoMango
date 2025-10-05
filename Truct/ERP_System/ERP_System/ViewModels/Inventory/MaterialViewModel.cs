using System.Collections.ObjectModel;
using System.Windows.Input;
using ERP_System.Models;
using ERP_System.Repositories.Inventory;
using ERP_System.Utilities;

namespace ERP_System.ViewModels.Inventory
{
    public class MaterialViewModel : BaseViewModel
    {
        private readonly MaterialRepository _materialRepository;

        private ObservableCollection<Material> _materials;
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set => SetProperty(ref _materials, value);
        }

        private Material _selectedMaterial;
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set => SetProperty(ref _selectedMaterial, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchMaterials();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }

        public MaterialViewModel()
        {
            _materialRepository = new MaterialRepository();
            Materials = new ObservableCollection<Material>();

            InitializeCommands();
            LoadData();
        }

        private void InitializeCommands()
        {
            LoadDataCommand = new RelayCommand(LoadData);
            AddCommand = new RelayCommand(AddMaterial);
            EditCommand = new RelayCommand(EditMaterial, CanEditDelete);
            DeleteCommand = new RelayCommand(DeleteMaterial, CanEditDelete);
            RefreshCommand = new RelayCommand(Refresh);
            ExportCommand = new RelayCommand(ExportToExcel);
        }

        private async void LoadData()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang tải danh sách vật tư...";

                var materials = await Task.Run(() => _materialRepository.GetMaterials());
                Materials = new ObservableCollection<Material>(materials);

                StatusMessage = $"Đã tải {materials.Count} vật tư";
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải danh sách vật tư: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddMaterial()
        {
            try
            {
                var newMaterial = new Material
                {
                    MaterialCode = _materialRepository.GenerateMaterialCode(),
                    MaterialName = "Vật tư mới",
                    UnitID = 1,
                    MaterialGroupID = 1,
                    MinStock = 0,
                    MaxStock = 0,
                    CostPrice = 0,
                    SellingPrice = 0,
                    IsActive = true
                };

                // TODO: Open material edit dialog
                // var dialog = new MaterialEditDialog(newMaterial);
                // if (dialog.ShowDialog() == true)
                // {
                //     if (_materialRepository.AddMaterial(newMaterial))
                //     {
                //         Materials.Add(newMaterial);
                //         ShowMessage("Thêm vật tư thành công");
                //     }
                // }

                ShowMessage("Chức năng thêm vật tư");
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi thêm vật tư: {ex.Message}");
            }
        }

        private void EditMaterial()
        {
            if (SelectedMaterial == null) return;

            try
            {
                // TODO: Open material edit dialog
                // var dialog = new MaterialEditDialog(SelectedMaterial);
                // if (dialog.ShowDialog() == true)
                // {
                //     if (_materialRepository.UpdateMaterial(SelectedMaterial))
                //     {
                //         ShowMessage("Cập nhật vật tư thành công");
                //     }
                // }

                ShowMessage("Chức năng sửa vật tư");
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi cập nhật vật tư: {ex.Message}");
            }
        }

        private void DeleteMaterial()
        {
            if (SelectedMaterial == null) return;

            if (ShowConfirmation($"Bạn có chắc chắn muốn xóa vật tư {SelectedMaterial.MaterialCode} - {SelectedMaterial.MaterialName}?"))
            {
                try
                {
                    if (_materialRepository.DeleteMaterial(SelectedMaterial.MaterialID))
                    {
                        Materials.Remove(SelectedMaterial);
                        ShowMessage("Xóa vật tư thành công");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Lỗi khi xóa vật tư: {ex.Message}");
                }
            }
        }

        private void SearchMaterials()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadData();
                return;
            }

            try
            {
                var results = _materialRepository.SearchMaterials(SearchText);
                Materials = new ObservableCollection<Material>(results);
                StatusMessage = $"Tìm thấy {results.Count} vật tư";
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tìm kiếm vật tư: {ex.Message}");
            }
        }

        private void Refresh()
        {
            LoadData();
        }

        private void ExportToExcel()
        {
            try
            {
                var exportPath = System.Configuration.ConfigurationManager.AppSettings["ExportPath"];
                var fileName = System.IO.Path.Combine(exportPath, $"Materials_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                if (ExportHelper.ExportToExcel(Materials.ToList(), fileName, "Danh sách vật tư"))
                {
                    ShowMessage($"Xuất dữ liệu thành công: {fileName}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi xuất Excel: {ex.Message}");
            }
        }

        private bool CanEditDelete()
        {
            return SelectedMaterial != null;
        }
    }
}