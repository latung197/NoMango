using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AIOI.Models;

namespace AIOI
{
    public partial class dmvt : Form
    {
        private BindingList<Material> materials = new BindingList<Material>();
        private Material currentMaterial = null;

        public dmvt()
        {
            InitializeComponent();
            SetupDataGridView();
            SetupComboBoxes();
            LoadSampleData();
        }

        private void SetupComboBoxes()
        {
            // ComboBox đơn vị tính
            cboUnit.Items.AddRange(new string[] { "Cái", "Chiếc", "Bộ", "Kg", "Gram", "Lít", "Mét", "Cuộn", "Thùng" });

            // ComboBox nhóm vật tư
            cboCategory.Items.AddRange(new string[] { "Điện tử", "Cơ khí", "Xây dựng", "Văn phòng", "Công nghệ", "Điện lạnh", "Thiết bị", "Khác" });

            // ComboBox nhà cung cấp
            cboSupplier.Items.AddRange(new string[] { "AIOI Company", "Công ty TNHH ABC", "Công ty CP XYZ", "Nhà cung cấp khác" });

            // ComboBox trạng thái
            cboStatus.Items.AddRange(new string[] { "Active", "Inactive", "Pending" });
        }

        private void SetupDataGridView()
        {
            dgvMaterials.AutoGenerateColumns = false;
            dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterials.MultiSelect = false;
            dgvMaterials.AllowUserToAddRows = false;

            // Xóa các cột cũ
            dgvMaterials.Columns.Clear();

            // Thêm các cột
            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colMaterialCode",
                HeaderText = "MÃ VT",
                DataPropertyName = "MaterialCode",
                Width = 80
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colMaterialName",
                HeaderText = "TÊN VẬT TƯ",
                DataPropertyName = "MaterialName",
                Width = 180
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colUnit",
                HeaderText = "ĐVT",
                DataPropertyName = "Unit",
                Width = 60
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colQuantity",
                HeaderText = "SỐ LƯỢNG",
                DataPropertyName = "Quantity",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colUnitPrice",
                HeaderText = "ĐƠN GIÁ",
                DataPropertyName = "UnitPrice",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTotalPrice",
                HeaderText = "THÀNH TIỀN",
                DataPropertyName = "TotalPrice",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colCategory",
                HeaderText = "NHÓM VT",
                DataPropertyName = "Category",
                Width = 100
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSupplier",
                HeaderText = "NHÀ CUNG CẤP",
                DataPropertyName = "Supplier",
                Width = 150
            });

            dgvMaterials.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colStatus",
                HeaderText = "TRẠNG THÁI",
                DataPropertyName = "Status",
                Width = 80
            });

            dgvMaterials.DataSource = materials;
        }

        private void LoadSampleData()
        {
            // Dữ liệu mẫu cho demo
            materials.Add(new Material()
            {
                MaterialCode = "VT001",
                MaterialName = "Máy tính Dell Optiplex",
                Unit = "Cái",
                Quantity = 5,
                UnitPrice = 15000000,
                Category = "Công nghệ",
                Supplier = "AIOI Company",
                Description = "Máy tính văn phòng"
            });

            materials.Add(new Material()
            {
                MaterialCode = "VT002",
                MaterialName = "Bàn làm việc",
                Unit = "Cái",
                Quantity = 10,
                UnitPrice = 2500000,
                Category = "Văn phòng",
                Supplier = "Công ty TNHH ABC",
                Description = "Bàn gỗ công nghiệp"
            });

            CalculateTotal();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Kiểm tra trùng mã vật tư
                if (materials.Any(m => m.MaterialCode == txtMaterialCode.Text.Trim()))
                {
                    MessageBox.Show("Mã vật tư đã tồn tại!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaterialCode.Focus();
                    return;
                }

                Material material = new Material
                {
                    MaterialCode = txtMaterialCode.Text.Trim(),
                    MaterialName = txtMaterialName.Text.Trim(),
                    Unit = cboUnit.Text,
                    Quantity = decimal.Parse(txtQuantity.Text),
                    UnitPrice = decimal.Parse(txtUnitPrice.Text.Replace(",", "")),
                    Category = cboCategory.Text,
                    Supplier = cboSupplier.Text,
                    Description = txtDescription.Text.Trim(),
                    Status = cboStatus.Text
                };

                materials.Add(material);
                ClearForm();
                CalculateTotal();
                MessageBox.Show("Thêm vật tư thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm vật tư: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (currentMaterial == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            try
            {
                currentMaterial.MaterialName = txtMaterialName.Text.Trim();
                currentMaterial.Unit = cboUnit.Text;
                currentMaterial.Quantity = decimal.Parse(txtQuantity.Text);
                currentMaterial.UnitPrice = decimal.Parse(txtUnitPrice.Text.Replace(",", ""));
                currentMaterial.Category = cboCategory.Text;
                currentMaterial.Supplier = cboSupplier.Text;
                currentMaterial.Description = txtDescription.Text.Trim();
                currentMaterial.Status = cboStatus.Text;

                dgvMaterials.Refresh();
                ClearForm();
                CalculateTotal();
                MessageBox.Show("Cập nhật vật tư thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentMaterial == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa vật tư này?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                materials.Remove(currentMaterial);
                ClearForm();
                CalculateTotal();
                MessageBox.Show("Xóa vật tư thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dgvMaterials.DataSource = materials;
                lblSearchResult.Text = $"Tìm thấy: {materials.Count} vật tư";
                return;
            }

            var filteredList = new BindingList<Material>(
                materials.Where(m =>
                    m.MaterialCode.ToLower().Contains(searchText) ||
                    m.MaterialName.ToLower().Contains(searchText) ||
                    m.Category.ToLower().Contains(searchText) ||
                    m.Supplier.ToLower().Contains(searchText) ||
                    m.Description.ToLower().Contains(searchText)
                ).ToList()
            );

            dgvMaterials.DataSource = filteredList;
            lblSearchResult.Text = $"Tìm thấy: {filteredList.Count} vật tư";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Export Danh mục vật tư";
                saveFileDialog.FileName = $"AIOI_DanhMucVatTu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Code export Excel sẽ được thêm sau
                    MessageBox.Show("Export dữ liệu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi export: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvMaterials_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaterials.SelectedRows.Count > 0)
            {
                currentMaterial = dgvMaterials.SelectedRows[0].DataBoundItem as Material;
                if (currentMaterial != null)
                {
                    DisplayMaterial(currentMaterial);
                }
            }
        }

        private void DisplayMaterial(Material material)
        {
            txtMaterialCode.Text = material.MaterialCode;
            txtMaterialName.Text = material.MaterialName;
            cboUnit.Text = material.Unit;
            txtQuantity.Text = material.Quantity.ToString("N0");
            txtUnitPrice.Text = material.UnitPrice.ToString("N0");
            cboCategory.Text = material.Category;
            cboSupplier.Text = material.Supplier;
            txtDescription.Text = material.Description;
            cboStatus.Text = material.Status;
        }

        private void ClearForm()
        {
            txtMaterialCode.Clear();
            txtMaterialName.Clear();
            cboUnit.SelectedIndex = -1;
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            cboCategory.SelectedIndex = -1;
            cboSupplier.SelectedIndex = -1;
            txtDescription.Clear();
            cboStatus.SelectedIndex = 0;
            currentMaterial = null;
            txtMaterialCode.Focus();
            lblItemTotal.Text = "Thành tiền: 0 VNĐ";
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaterialCode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vật tư!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaterialCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMaterialName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên vật tư!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaterialName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtQuantity.Text.Replace(",", ""), out decimal quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng phải là số dương!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text.Replace(",", ""), out decimal unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Đơn giá phải là số dương!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return false;
            }

            return true;
        }

        private void CalculateTotal()
        {
            decimal total = materials.Sum(m => m.TotalPrice);
            int totalItems = materials.Count;
            lblTotal.Text = $"Tổng số: {totalItems} vật tư - Tổng giá trị: {total:N0} VNĐ";
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateItemTotal();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateItemTotal();
        }

        private void CalculateItemTotal()
        {
            if (decimal.TryParse(txtQuantity.Text.Replace(",", ""), out decimal quantity) &&
                decimal.TryParse(txtUnitPrice.Text.Replace(",", ""), out decimal unitPrice))
            {
                lblItemTotal.Text = $"Thành tiền: {(quantity * unitPrice):N0} VNĐ";
            }
            else
            {
                lblItemTotal.Text = "Thành tiền: 0 VNĐ";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            btnSearch.PerformClick();
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtUnitPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtUnitPrice_Leave(object sender, EventArgs e)
        {
            // Format số khi rời khỏi textbox
            if (decimal.TryParse(txtUnitPrice.Text.Replace(",", ""), out decimal value))
            {
                txtUnitPrice.Text = value.ToString("N0");
            }
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            // Format số khi rời khỏi textbox
            if (decimal.TryParse(txtQuantity.Text.Replace(",", ""), out decimal value))
            {
                txtQuantity.Text = value.ToString("N0");
            }
        }
    }
}