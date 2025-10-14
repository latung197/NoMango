using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AIOI.Models;

namespace AIOI.View
{
    public partial class frmChonVatTu : Form
    {
        public List<Material> VatTuDaChon { get; private set; }
        private List<Material> danhSachVatTu;
        private BindingList<Material> filteredVatTu;
        private List<Material> selectedVatTu;

        public frmChonVatTu()
        {
            InitializeComponent();
            VatTuDaChon = new List<Material>();
            selectedVatTu = new List<Material>();
            LoadDanhSachVatTu();
            SetupDataGridView();
        }

        private void LoadDanhSachVatTu()
        {
            // Tải danh sách vật tư từ nguồn dữ liệu
            danhSachVatTu = new List<Material>
            {
                new Material
                {
                    MaterialCode = "VT001",
                    MaterialName = "Máy tính Dell Optiplex",
                    Unit = "Cái",
                    UnitPrice = 15000000,
                    Category = "Công nghệ",
                    Supplier = "AIOI Company"
                },
                new Material
                {
                    MaterialCode = "VT002",
                    MaterialName = "Bàn làm việc gỗ",
                    Unit = "Cái",
                    UnitPrice = 2500000,
                    Category = "Văn phòng",
                    Supplier = "Công ty ABC"
                },
                new Material
                {
                    MaterialCode = "VT003",
                    MaterialName = "Ghế xoay văn phòng",
                    Unit = "Cái",
                    UnitPrice = 1500000,
                    Category = "Văn phòng",
                    Supplier = "Công ty XYZ"
                },
                new Material
                {
                    MaterialCode = "VT004",
                    MaterialName = "Màn hình LCD 24 inch",
                    Unit = "Cái",
                    UnitPrice = 4500000,
                    Category = "Công nghệ",
                    Supplier = "AIOI Company"
                },
                new Material
                {
                    MaterialCode = "VT005",
                    MaterialName = "Bàn phím cơ",
                    Unit = "Cái",
                    UnitPrice = 800000,
                    Category = "Công nghệ",
                    Supplier = "Công ty ABC"
                }
            };

            filteredVatTu = new BindingList<Material>(danhSachVatTu);
            dgvVatTu.DataSource = filteredVatTu;
        }

        private void SetupDataGridView()
        {
            dgvVatTu.AutoGenerateColumns = false;
            dgvVatTu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVatTu.MultiSelect = true; // Cho phép chọn nhiều
            dgvVatTu.AllowUserToAddRows = false;

            dgvVatTu.Columns.Clear();

            // Thêm cột checkbox để chọn
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colChon";
            checkBoxColumn.HeaderText = "Chọn";
            checkBoxColumn.Width = 50;
            dgvVatTu.Columns.Add(checkBoxColumn);

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colMaVatTu",
                HeaderText = "MÃ VT",
                DataPropertyName = "MaterialCode",
                Width = 80
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTenVatTu",
                HeaderText = "TÊN VẬT TƯ",
                DataPropertyName = "MaterialName",
                Width = 180
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "Unit",
                Width = 60
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDonGia",
                HeaderText = "ĐƠN GIÁ",
                DataPropertyName = "UnitPrice",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNhom",
                HeaderText = "NHÓM",
                DataPropertyName = "Category",
                Width = 100
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNhaCungCap",
                HeaderText = "NHÀ CUNG CẤP",
                DataPropertyName = "Supplier",
                Width = 150
            });
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            // Lấy danh sách vật tư đã chọn
            VatTuDaChon.Clear();
            foreach (DataGridViewRow row in dgvVatTu.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colChon"] as DataGridViewCheckBoxCell;
                if (chk.Value != null && (bool)chk.Value)
                {
                    var vatTu = row.DataBoundItem as Material;
                    if (vatTu != null)
                    {
                        VatTuDaChon.Add(vatTu);
                    }
                }
            }

            if (VatTuDaChon.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một vật tư!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                filteredVatTu = new BindingList<Material>(danhSachVatTu);
            }
            else
            {
                var filteredList = danhSachVatTu.Where(v =>
                    v.MaterialCode.ToLower().Contains(searchText) ||
                    v.MaterialName.ToLower().Contains(searchText) ||
                    v.Category.ToLower().Contains(searchText) ||
                    v.Supplier.ToLower().Contains(searchText)
                ).ToList();

                filteredVatTu = new BindingList<Material>(filteredList);
            }

            dgvVatTu.DataSource = filteredVatTu;
            lblKetQua.Text = $"Tìm thấy: {filteredVatTu.Count} vật tư";
        }

        private void dgvVatTu_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVatTu.SelectedRows.Count > 0)
            {
                var vatTu = dgvVatTu.SelectedRows[0].DataBoundItem as Material;
                if (vatTu != null)
                {
                    DisplayVatTuInfo(vatTu);
                }
            }
        }

        private void DisplayVatTuInfo(Material vatTu)
        {
            lblMaVatTu.Text = vatTu.MaterialCode;
            lblTenVatTu.Text = vatTu.MaterialName;
            lblDonViTinh.Text = vatTu.Unit;
            lblDonGia.Text = vatTu.UnitPrice.ToString("N0");
            lblNhom.Text = vatTu.Category;
            lblNhaCungCap.Text = vatTu.Supplier;
        }

        private void dgvVatTu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex != 0) // Không phải cột checkbox
            {
                // Toggle trạng thái chọn khi double click
                DataGridViewCheckBoxCell chk = dgvVatTu.Rows[e.RowIndex].Cells["colChon"] as DataGridViewCheckBoxCell;
                if (chk.Value == null || !(bool)chk.Value)
                {
                    chk.Value = true;
                }
                else
                {
                    chk.Value = false;
                }
            }
        }

        private void btnChonTatCa_Click(object sender, EventArgs e)
        {
            // Chọn tất cả vật tư
            foreach (DataGridViewRow row in dgvVatTu.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colChon"] as DataGridViewCheckBoxCell;
                chk.Value = true;
            }
        }

        private void btnBoChonTatCa_Click(object sender, EventArgs e)
        {
            // Bỏ chọn tất cả vật tư
            foreach (DataGridViewRow row in dgvVatTu.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colChon"] as DataGridViewCheckBoxCell;
                chk.Value = false;
            }
        }

        private void dgvVatTu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Xử lý sự kiện click trên checkbox
            if (e.ColumnIndex == 0 && e.RowIndex >= 0) // Cột checkbox
            {
                dgvVatTu.CommitEdit(DataGridViewDataErrorContexts.Commit);
                UpdateSelectedCount();
            }
        }

        private void UpdateSelectedCount()
        {
            int count = 0;
            foreach (DataGridViewRow row in dgvVatTu.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colChon"] as DataGridViewCheckBoxCell;
                if (chk.Value != null && (bool)chk.Value)
                {
                    count++;
                }
            }
            lblSoLuongChon.Text = $"Đã chọn: {count} vật tư";
        }

        private void dgvVatTu_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) // Cột checkbox
            {
                UpdateSelectedCount();
            }
        }
    }
}