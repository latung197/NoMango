using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AIOI.Models;

namespace AIOI.View
{
    public partial class frmPhieuNhap_ChiTiet : Form
    {
        public PhieuNhapKho PhieuNhap { get; private set; }
        private BindingList<ChiTietPhieuNhap> chiTietPhieu;
        private bool isEditMode = false;

        public frmPhieuNhap_ChiTiet()
        {
            InitializeComponent();
            InitializeNewPhieu();
            SetupDataGridView();
        }

        public frmPhieuNhap_ChiTiet(PhieuNhapKho phieuNhap)
        {
            InitializeComponent();
            PhieuNhap = phieuNhap;
            isEditMode = true;
            SetupDataGridView();
            LoadPhieuData();
        }

        private void InitializeNewPhieu()
        {
            PhieuNhap = new PhieuNhapKho();
            PhieuNhap.SoPhieu = GenerateSoPhieu();
            chiTietPhieu = new BindingList<ChiTietPhieuNhap>();
            txtSoPhieu.Text = PhieuNhap.SoPhieu;
            dtpNgayNhap.Value = DateTime.Now;
        }

        private void LoadPhieuData()
        {
            chiTietPhieu = new BindingList<ChiTietPhieuNhap>(PhieuNhap.ChiTiet);
            dgvChiTiet.DataSource = chiTietPhieu;

            txtSoPhieu.Text = PhieuNhap.SoPhieu;
            dtpNgayNhap.Value = PhieuNhap.NgayNhap;
            txtNhaCungCap.Text = PhieuNhap.NhaCungCap;
            txtKhoNhap.Text = PhieuNhap.KhoNhap;
            txtNguoiGiao.Text = PhieuNhap.NguoiGiao;
            txtLyDoNhap.Text = PhieuNhap.LyDoNhap;
            txtGhiChu.Text = PhieuNhap.GhiChu;
            cboTrangThai.Text = PhieuNhap.TrangThai;

            CalculateTotal();
        }

        private string GenerateSoPhieu()
        {
            return $"PN{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void SetupDataGridView()
        {
            dgvChiTiet.AutoGenerateColumns = false;
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTiet.AllowUserToAddRows = false;

            dgvChiTiet.Columns.Clear();

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colMaVatTu",
                HeaderText = "MÃ VẬT TƯ",
                DataPropertyName = "MaVatTu",
                Width = 100
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTenVatTu",
                HeaderText = "TÊN VẬT TƯ",
                DataPropertyName = "TenVatTu",
                Width = 150
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 60
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSoLuong",
                HeaderText = "SỐ LƯỢNG",
                DataPropertyName = "SoLuong",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colDonGia",
                HeaderText = "ĐƠN GIÁ",
                DataPropertyName = "DonGia",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colThanhTien",
                HeaderText = "THÀNH TIỀN",
                DataPropertyName = "ThanhTien",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colViTri",
                HeaderText = "VỊ TRÍ",
                DataPropertyName = "ViTri",
                Width = 100
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colGhiChu",
                HeaderText = "GHI CHÚ",
                DataPropertyName = "GhiChu",
                Width = 120
            });

            if (!isEditMode)
            {
                dgvChiTiet.DataSource = chiTietPhieu;
            }
        }

        private void btnThemChiTiet_Click(object sender, EventArgs e)
        {
            var formChonVatTu = new frmChonVatTu();
            if (formChonVatTu.ShowDialog() == DialogResult.OK)
            {
                foreach (var vatTu in formChonVatTu.VatTuDaChon)
                {
                    // Kiểm tra xem vật tư đã tồn tại trong chi tiết chưa
                    if (!chiTietPhieu.Any(ct => ct.MaVatTu == vatTu.MaterialCode))
                    {
                        var chiTiet = new ChiTietPhieuNhap()
                        {
                            MaVatTu = vatTu.MaterialCode,
                            TenVatTu = vatTu.MaterialName,
                            DonViTinh = vatTu.Unit,
                            DonGia = vatTu.UnitPrice,
                            SoLuong = 1, // Mặc định số lượng là 1
                            ViTri = ""
                        };
                        chiTiet.TinhThanhTien();

                        chiTietPhieu.Add(chiTiet);
                    }
                    else
                    {
                        MessageBox.Show($"Vật tư {vatTu.MaterialCode} - {vatTu.MaterialName} đã có trong phiếu!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                CalculateTotal();
            }
        }

        private void btnSuaChiTiet_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn chi tiết cần sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chiTiet = dgvChiTiet.SelectedRows[0].DataBoundItem as ChiTietPhieuNhap;
            //var formSuaChiTiet = new frmSuaChiTietPhieu(chiTiet);
            //if (formSuaChiTiet.ShowDialog() == DialogResult.OK)
            //{
            //    dgvChiTiet.Refresh();
            //    CalculateTotal();
            //}
        }

        private void btnXoaChiTiet_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn chi tiết cần xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chiTiet = dgvChiTiet.SelectedRows[0].DataBoundItem as ChiTietPhieuNhap;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa chi tiết này?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                chiTietPhieu.Remove(chiTiet);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            decimal total = chiTietPhieu.Sum(ct => ct.ThanhTien);
            lblTongTien.Text = $"TỔNG TIỀN: {total:N0} VNĐ";
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            if (chiTietPhieu.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một chi tiết vật tư!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cập nhật thông tin phiếu
            PhieuNhap.SoPhieu = txtSoPhieu.Text.Trim();
            PhieuNhap.NgayNhap = dtpNgayNhap.Value;
            PhieuNhap.NhaCungCap = txtNhaCungCap.Text.Trim();
            PhieuNhap.KhoNhap = txtKhoNhap.Text.Trim();
            PhieuNhap.NguoiGiao = txtNguoiGiao.Text.Trim();
            PhieuNhap.LyDoNhap = txtLyDoNhap.Text.Trim();
            PhieuNhap.GhiChu = txtGhiChu.Text.Trim();
            PhieuNhap.TrangThai = cboTrangThai.Text;
            PhieuNhap.TongTien = chiTietPhieu.Sum(ct => ct.ThanhTien);

            // Cập nhật chi tiết
            PhieuNhap.ChiTiet.Clear();
            foreach (var ct in chiTietPhieu)
            {
                PhieuNhap.ChiTiet.Add(ct);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtSoPhieu.Text))
            {
                MessageBox.Show("Vui lòng nhập số phiếu!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoPhieu.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNhaCungCap.Text))
            {
                MessageBox.Show("Vui lòng nhập nhà cung cấp!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNhaCungCap.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtKhoNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập kho nhập!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKhoNhap.Focus();
                return false;
            }

            return true;
        }

        private void dgvChiTiet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Tự động tính thành tiền khi chỉnh sửa số lượng hoặc đơn giá
            if (e.ColumnIndex == dgvChiTiet.Columns["colSoLuong"].Index ||
                e.ColumnIndex == dgvChiTiet.Columns["colDonGia"].Index)
            {
                var chiTiet = dgvChiTiet.Rows[e.RowIndex].DataBoundItem as ChiTietPhieuNhap;
                if (chiTiet != null)
                {
                    chiTiet.TinhThanhTien();
                    dgvChiTiet.Refresh();
                    CalculateTotal();
                }
            }
        }

        private void frmPhieuNhap_ChiTiet_Load(object sender, EventArgs e)
        {

        }
    }
}