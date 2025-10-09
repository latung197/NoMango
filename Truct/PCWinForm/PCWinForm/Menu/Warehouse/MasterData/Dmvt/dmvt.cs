using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCWinForm.Menu.Warehouse.MasterData.Dmvt
{
    public partial class dmvt : Form
    {
        private BindingList<dmvt> danhSachVatTu;
        private dmvt vatTuDangChon = null;
        private bool isEditMode = false;

        public dmvt()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadSampleData();
        }

        private void SetupDataGridView()
        {
            dgvDanhSachVatTu.AutoGenerateColumns = false;
            dgvDanhSachVatTu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSachVatTu.MultiSelect = false;
            dgvDanhSachVatTu.AllowUserToAddRows = false;

            // Xóa các cột cũ
            dgvDanhSachVatTu.Columns.Clear();

            // Thêm các cột
            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colMaVT",
                HeaderText = "MÃ VT",
                DataPropertyName = "mavt",
                Width = 100
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTenVT",
                HeaderText = "TÊN VẬT TƯ",
                DataPropertyName = "tenvt",
                Width = 200
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDVT",
                HeaderText = "ĐVT",
                DataPropertyName = "dvt",
                Width = 60
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSLTon",
                HeaderText = "SL TỒN",
                DataPropertyName = "slton",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDonGia",
                HeaderText = "ĐƠN GIÁ",
                DataPropertyName = "dongia",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colThanhTien",
                HeaderText = "THÀNH TIỀN",
                DataPropertyName = "thanhtien",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNhomVT",
                HeaderText = "NHÓM VT",
                DataPropertyName = "nhomvt",
                Width = 100
            });

            dgvDanhSachVatTu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTrangThai",
                HeaderText = "TRẠNG THÁI",
                DataPropertyName = "trangthai",
                Width = 80
            });

            dgvDanhSachVatTu.DataSource = danhSachVatTu;
        }

        private void LoadSampleData()
        {
            danhSachVatTu = new BindingList<dmvt>
            {
                new dmvt
                {
                    mavt = "VT001",
                    tenvt = "Máy tính Dell Optiplex 7090",
                    dvt = "Cái",
                    slton = 5,
                    dongia = 15000000,
                    nhomvt = "Công nghệ",
                    nhacc = "Công ty AIOI",
                    trangthai = true
                },
                new dmvt
                {
                    mavt = "VT002",
                    tenvt = "Bàn làm việc Hòa Phát",
                    dvt = "Cái",
                    slton = 10,
                    dongia = 2500000,
                    nhomvt = "Văn phòng",
                    nhacc = "Công ty ABC",
                    trangthai = true
                },
                new dmvt
                {
                    mavt = "VT003",
                    tenvt = "Ghế xoay văn phòng",
                    dvt = "Cái",
                    slton = 8,
                    dongia = 1500000,
                    nhomvt = "Văn phòng",
                    nhacc = "Công ty XYZ",
                    trangthai = false
                }
            };

            dgvDanhSachVatTu.DataSource = danhSachVatTu;
            CalculateTotals();
        }

        private void dgvDanhSachVatTu_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSachVatTu.SelectedRows.Count > 0)
            {
                vatTuDangChon = dgvDanhSachVatTu.SelectedRows[0].DataBoundItem as dmvt;
                if (vatTuDangChon != null)
                {
                    DisplayVatTuInfo(vatTuDangChon);
                }
            }
        }

        private void DisplayVatTuInfo(dmvt vatTu)
        {
            // Hiển thị thông tin vật tư đang chọn
            lblMaVT.Text = vatTu.mavt;
            lblTenVT.Text = vatTu.tenvt;
            lblDVT.Text = vatTu.dvt;
            lblSLTon.Text = vatTu.slton?.ToString("N0") ?? "0";
            lblDonGia.Text = vatTu.dongia?.ToString("N0") ?? "0";
            lblThanhTien.Text = vatTu.thanhtien?.ToString("N0") ?? "0";
            lblNhomVT.Text = vatTu.nhomvt;
            lblTrangThai.Text = vatTu.trangthai == true ? "Đang hoạt động" : "Ngừng hoạt động";
            lblTrangThai.ForeColor = vatTu.trangthai == true ? Color.Green : Color.Red;
        }

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            isEditMode = false;
            ShowEditForm(null);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (vatTuDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isEditMode = true;
            ShowEditForm(vatTuDangChon);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (vatTuDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa vật tư {vatTuDangChon.mavt} - {vatTuDangChon.tenvt}?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                danhSachVatTu.Remove(vatTuDangChon);
                ClearSelection();
                CalculateTotals();
                MessageBox.Show("Xóa vật tư thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowEditForm(dmvt vatTu)
        {
            using (var editForm = new frmEditVatTu(vatTu, isEditMode))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    var result = editForm.VatTu;

                    if (isEditMode)
                    {
                        // Cập nhật vật tư hiện có
                        var existing = danhSachVatTu.FirstOrDefault(v => v.mavt == result.mavt);
                        if (existing != null)
                        {
                            existing.tenvt = result.tenvt;
                            existing.dvt = result.dvt;
                            existing.slton = result.slton;
                            existing.dongia = result.dongia;
                            existing.nhomvt = result.nhomvt;
                            existing.nhacc = result.nhacc;
                            existing.ghichu = result.ghichu;
                            existing.trangthai = result.trangthai;
                        }
                    }
                    else
                    {
                        // Thêm vật tư mới
                        // Kiểm tra trùng mã
                        if (danhSachVatTu.Any(v => v.mavt == result.mavt))
                        {
                            MessageBox.Show("Mã vật tư đã tồn tại!", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        danhSachVatTu.Add(result);
                    }

                    dgvDanhSachVatTu.Refresh();
                    CalculateTotals();

                    string message = isEditMode ? "Cập nhật vật tư thành công!" : "Thêm vật tư mới thành công!";
                    MessageBox.Show(message, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClearSelection()
        {
            vatTuDangChon = null;
            lblMaVT.Text = "";
            lblTenVT.Text = "";
            lblDVT.Text = "";
            lblSLTon.Text = "";
            lblDonGia.Text = "";
            lblThanhTien.Text = "";
            lblNhomVT.Text = "";
            lblTrangThai.Text = "";
        }

        private void CalculateTotals()
        {
            int totalItems = danhSachVatTu.Count;
            decimal totalValue = danhSachVatTu.Sum(v => v.thanhtien ?? 0);
            int activeItems = danhSachVatTu.Count(v => v.trangthai == true);

            lblTongHop.Text = $"Tổng số: {totalItems} vật tư - Đang hoạt động: {activeItems} - Tổng giá trị: {totalValue:N0} VNĐ";
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dgvDanhSachVatTu.DataSource = danhSachVatTu;
                return;
            }

            var filteredList = new BindingList<dmvt>(
                danhSachVatTu.Where(v =>
                    v.mavt.ToLower().Contains(searchText) ||
                    v.tenvt.ToLower().Contains(searchText) ||
                    (v.nhomvt?.ToLower().Contains(searchText) == true) ||
                    (v.nhacc?.ToLower().Contains(searchText) == true)
                ).ToList()
            );

            dgvDanhSachVatTu.DataSource = filteredList;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            ClearSelection();
            dgvDanhSachVatTu.DataSource = danhSachVatTu;
        }

        private void dgvDanhSachVatTu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSua.PerformClick();
            }
        }
    }
}
