using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AIOI.Models;

namespace AIOI.View
{
    public partial class frmPhieuNhap_DanhSach : Form
    {
        private BindingList<PhieuNhapKho> danhSachPhieu = new BindingList<PhieuNhapKho>();
        private PhieuNhapKho phieuSelected = null;

        public frmPhieuNhap_DanhSach()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadSampleData();
        }

        private void SetupDataGridView()
        {
            dgvDanhSachPhieu.AutoGenerateColumns = false;
            dgvDanhSachPhieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSachPhieu.MultiSelect = false;
            dgvDanhSachPhieu.AllowUserToAddRows = false;

            // Xóa các cột cũ
            dgvDanhSachPhieu.Columns.Clear();

            // Thêm các cột
            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colSoPhieu",
                HeaderText = "SỐ PHIẾU",
                DataPropertyName = "SoPhieu",
                Width = 100
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNgayNhap",
                HeaderText = "NGÀY NHẬP",
                DataPropertyName = "NgayNhap",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "dd/MM/yyyy" }
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNhaCungCap",
                HeaderText = "NHÀ CUNG CẤP",
                DataPropertyName = "NhaCungCap",
                Width = 150
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colKhoNhap",
                HeaderText = "KHO NHẬP",
                DataPropertyName = "KhoNhap",
                Width = 120
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTongTien",
                HeaderText = "TỔNG TIỀN",
                DataPropertyName = "TongTien",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "N0" }
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colTrangThai",
                HeaderText = "TRẠNG THÁI",
                DataPropertyName = "TrangThai",
                Width = 100
            });

            dgvDanhSachPhieu.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "colNguoiTao",
                HeaderText = "NGƯỜI TẠO",
                DataPropertyName = "NguoiTao",
                Width = 100
            });

            dgvDanhSachPhieu.DataSource = danhSachPhieu;
        }

        private void LoadSampleData()
        {
            // Dữ liệu mẫu
            var phieu1 = new PhieuNhapKho()
            {
                SoPhieu = "PN001",
                NgayNhap = DateTime.Now.AddDays(-5),
                NhaCungCap = "Công ty ABC",
                KhoNhap = "Kho chính",
                NguoiGiao = "Nguyễn Văn A",
                LyDoNhap = "Nhập hàng định kỳ",
                TongTien = 15000000,
                TrangThai = "Đã duyệt"
            };
            phieu1.ChiTiet.Add(new ChiTietPhieuNhap()
            {
                MaVatTu = "VT001",
                TenVatTu = "Máy tính Dell",
                DonViTinh = "Cái",
                SoLuong = 5,
                DonGia = 10000000,
                ViTri = "Kệ A1"
            });
            phieu1.ChiTiet[0].TinhThanhTien();

            var phieu2 = new PhieuNhapKho()
            {
                SoPhieu = "PN002",
                NgayNhap = DateTime.Now.AddDays(-2),
                NhaCungCap = "Công ty XYZ",
                KhoNhap = "Kho phụ",
                NguoiGiao = "Trần Thị B",
                LyDoNhap = "Nhập bổ sung",
                TongTien = 8000000,
                TrangThai = "Mới"
            };
            phieu2.ChiTiet.Add(new ChiTietPhieuNhap()
            {
                MaVatTu = "VT002",
                TenVatTu = "Bàn làm việc",
                DonViTinh = "Cái",
                SoLuong = 10,
                DonGia = 800000,
                ViTri = "Kệ B2"
            });
            phieu2.ChiTiet[0].TinhThanhTien();

            danhSachPhieu.Add(phieu1);
            danhSachPhieu.Add(phieu2);

            CalculateTotals();
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            var formChiTiet = new frmPhieuNhap_ChiTiet();
            if (formChiTiet.ShowDialog() == DialogResult.OK)
            {
                danhSachPhieu.Add(formChiTiet.PhieuNhap);
                CalculateTotals();
                MessageBox.Show("Tạo phiếu nhập kho thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            if (phieuSelected == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập kho để xem chi tiết!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var formChiTiet = new frmPhieuNhap_ChiTiet(phieuSelected);
            formChiTiet.ShowDialog();

            // Refresh data if needed
            dgvDanhSachPhieu.Refresh();
            CalculateTotals();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (phieuSelected == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập kho để sửa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var formChiTiet = new frmPhieuNhap_ChiTiet(phieuSelected);
            if (formChiTiet.ShowDialog() == DialogResult.OK)
            {
                dgvDanhSachPhieu.Refresh();
                CalculateTotals();
                MessageBox.Show("Cập nhật phiếu nhập kho thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (phieuSelected == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập kho để xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu {phieuSelected.SoPhieu}?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                danhSachPhieu.Remove(phieuSelected);
                CalculateTotals();
                MessageBox.Show("Xóa phiếu nhập kho thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnInPhieu_Click(object sender, EventArgs e)
        {
            if (phieuSelected == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập kho để in!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Code in phiếu sẽ được thêm sau
            MessageBox.Show($"In phiếu nhập kho {phieuSelected.SoPhieu}", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvDanhSachPhieu_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSachPhieu.SelectedRows.Count > 0)
            {
                phieuSelected = dgvDanhSachPhieu.SelectedRows[0].DataBoundItem as PhieuNhapKho;
                if (phieuSelected != null)
                {
                    DisplayPhieuInfo(phieuSelected);
                }
            }
        }

        private void DisplayPhieuInfo(PhieuNhapKho phieu)
        {
            lblSoPhieu.Text = phieu.SoPhieu;
            lblNgayNhap.Text = phieu.NgayNhap.ToString("dd/MM/yyyy");
            lblNhaCungCap.Text = phieu.NhaCungCap;
            lblTongTien.Text = phieu.TongTien.ToString("N0");
            lblTrangThai.Text = phieu.TrangThai;
        }

        private void CalculateTotals()
        {
            int totalPhieu = danhSachPhieu.Count;
            decimal totalValue = danhSachPhieu.Sum(p => p.TongTien);
            lblTongHop.Text = $"Tổng số: {totalPhieu} phiếu - Tổng giá trị: {totalValue:N0} VNĐ";
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtTimKiem.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dgvDanhSachPhieu.DataSource = danhSachPhieu;
                return;
            }

            var filteredList = new BindingList<PhieuNhapKho>(
                danhSachPhieu.Where(p =>
                    p.SoPhieu.ToLower().Contains(searchText) ||
                    p.NhaCungCap.ToLower().Contains(searchText) ||
                    p.KhoNhap.ToLower().Contains(searchText) ||
                    p.NguoiGiao.ToLower().Contains(searchText) ||
                    p.LyDoNhap.ToLower().Contains(searchText)
                ).ToList()
            );

            dgvDanhSachPhieu.DataSource = filteredList;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Export Danh sách phiếu nhập kho";
                saveFileDialog.FileName = $"DS_PhieuNhapKho_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
    }
}