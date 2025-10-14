namespace AIOI.View
{
    partial class frmPhieuNhap_ChiTiet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelChiTiet = new System.Windows.Forms.Panel();
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.panelChiTietButtons = new System.Windows.Forms.Panel();
            this.btnXoaChiTiet = new System.Windows.Forms.Button();
            this.btnSuaChiTiet = new System.Windows.Forms.Button();
            this.btnThemChiTiet = new System.Windows.Forms.Button();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.panelThongTin = new System.Windows.Forms.Panel();
            this.cboTrangThai = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtGhiChu = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtLyDoNhap = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtNguoiGiao = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtKhoNhap = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNhaCungCap = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpNgayNhap = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSoPhieu = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnLuu = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            this.panelChiTietButtons.SuspendLayout();
            this.panelThongTin.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.SteelBlue;
            this.panelHeader.Controls.Add(this.label1);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(750, 49);
            this.panelHeader.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "CHI TIẾT PHIẾU NHẬP";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelChiTiet);
            this.panelMain.Controls.Add(this.panelThongTin);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 49);
            this.panelMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(750, 438);
            this.panelMain.TabIndex = 1;
            // 
            // panelChiTiet
            // 
            this.panelChiTiet.Controls.Add(this.dgvChiTiet);
            this.panelChiTiet.Controls.Add(this.panelChiTietButtons);
            this.panelChiTiet.Controls.Add(this.lblTongTien);
            this.panelChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChiTiet.Location = new System.Drawing.Point(300, 0);
            this.panelChiTiet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelChiTiet.Name = "panelChiTiet";
            this.panelChiTiet.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelChiTiet.Size = new System.Drawing.Size(450, 438);
            this.panelChiTiet.TabIndex = 1;
            // 
            // dgvChiTiet
            // 
            this.dgvChiTiet.AllowUserToAddRows = false;
            this.dgvChiTiet.AllowUserToDeleteRows = false;
            this.dgvChiTiet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTiet.BackgroundColor = System.Drawing.Color.White;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTiet.Location = new System.Drawing.Point(8, 40);
            this.dgvChiTiet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.RowHeadersWidth = 51;
            this.dgvChiTiet.RowTemplate.Height = 24;
            this.dgvChiTiet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChiTiet.Size = new System.Drawing.Size(434, 349);
            this.dgvChiTiet.TabIndex = 2;
            this.dgvChiTiet.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvChiTiet_CellEndEdit);
            // 
            // panelChiTietButtons
            // 
            this.panelChiTietButtons.Controls.Add(this.btnXoaChiTiet);
            this.panelChiTietButtons.Controls.Add(this.btnSuaChiTiet);
            this.panelChiTietButtons.Controls.Add(this.btnThemChiTiet);
            this.panelChiTietButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelChiTietButtons.Location = new System.Drawing.Point(8, 8);
            this.panelChiTietButtons.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelChiTietButtons.Name = "panelChiTietButtons";
            this.panelChiTietButtons.Size = new System.Drawing.Size(434, 32);
            this.panelChiTietButtons.TabIndex = 1;
            // 
            // btnXoaChiTiet
            // 
            this.btnXoaChiTiet.BackColor = System.Drawing.Color.Crimson;
            this.btnXoaChiTiet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaChiTiet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXoaChiTiet.ForeColor = System.Drawing.Color.White;
            this.btnXoaChiTiet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnXoaChiTiet.Location = new System.Drawing.Point(152, 4);
            this.btnXoaChiTiet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnXoaChiTiet.Name = "btnXoaChiTiet";
            this.btnXoaChiTiet.Size = new System.Drawing.Size(68, 24);
            this.btnXoaChiTiet.TabIndex = 2;
            this.btnXoaChiTiet.Text = "Xóa";
            this.btnXoaChiTiet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnXoaChiTiet.UseVisualStyleBackColor = false;
            this.btnXoaChiTiet.Click += new System.EventHandler(this.btnXoaChiTiet_Click);
            // 
            // btnSuaChiTiet
            // 
            this.btnSuaChiTiet.BackColor = System.Drawing.Color.Orange;
            this.btnSuaChiTiet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuaChiTiet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSuaChiTiet.ForeColor = System.Drawing.Color.White;
            this.btnSuaChiTiet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSuaChiTiet.Location = new System.Drawing.Point(80, 4);
            this.btnSuaChiTiet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSuaChiTiet.Name = "btnSuaChiTiet";
            this.btnSuaChiTiet.Size = new System.Drawing.Size(68, 24);
            this.btnSuaChiTiet.TabIndex = 1;
            this.btnSuaChiTiet.Text = "Sửa";
            this.btnSuaChiTiet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSuaChiTiet.UseVisualStyleBackColor = false;
            this.btnSuaChiTiet.Click += new System.EventHandler(this.btnSuaChiTiet_Click);
            // 
            // btnThemChiTiet
            // 
            this.btnThemChiTiet.BackColor = System.Drawing.Color.SeaGreen;
            this.btnThemChiTiet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemChiTiet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemChiTiet.ForeColor = System.Drawing.Color.White;
            this.btnThemChiTiet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThemChiTiet.Location = new System.Drawing.Point(8, 4);
            this.btnThemChiTiet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnThemChiTiet.Name = "btnThemChiTiet";
            this.btnThemChiTiet.Size = new System.Drawing.Size(68, 24);
            this.btnThemChiTiet.TabIndex = 0;
            this.btnThemChiTiet.Text = "Thêm";
            this.btnThemChiTiet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnThemChiTiet.UseVisualStyleBackColor = false;
            this.btnThemChiTiet.Click += new System.EventHandler(this.btnThemChiTiet_Click);
            // 
            // lblTongTien
            // 
            this.lblTongTien.BackColor = System.Drawing.Color.Gold;
            this.lblTongTien.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTongTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongTien.Location = new System.Drawing.Point(8, 389);
            this.lblTongTien.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(434, 41);
            this.lblTongTien.TabIndex = 0;
            this.lblTongTien.Text = "TỔNG TIỀN: 0 VNĐ";
            this.lblTongTien.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelThongTin
            // 
            this.panelThongTin.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelThongTin.Controls.Add(this.cboTrangThai);
            this.panelThongTin.Controls.Add(this.label11);
            this.panelThongTin.Controls.Add(this.txtGhiChu);
            this.panelThongTin.Controls.Add(this.label10);
            this.panelThongTin.Controls.Add(this.txtLyDoNhap);
            this.panelThongTin.Controls.Add(this.label9);
            this.panelThongTin.Controls.Add(this.txtNguoiGiao);
            this.panelThongTin.Controls.Add(this.label8);
            this.panelThongTin.Controls.Add(this.txtKhoNhap);
            this.panelThongTin.Controls.Add(this.label7);
            this.panelThongTin.Controls.Add(this.txtNhaCungCap);
            this.panelThongTin.Controls.Add(this.label6);
            this.panelThongTin.Controls.Add(this.dtpNgayNhap);
            this.panelThongTin.Controls.Add(this.label5);
            this.panelThongTin.Controls.Add(this.txtSoPhieu);
            this.panelThongTin.Controls.Add(this.label4);
            this.panelThongTin.Controls.Add(this.label3);
            this.panelThongTin.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelThongTin.Location = new System.Drawing.Point(0, 0);
            this.panelThongTin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelThongTin.Name = "panelThongTin";
            this.panelThongTin.Padding = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.panelThongTin.Size = new System.Drawing.Size(300, 438);
            this.panelThongTin.TabIndex = 0;
            // 
            // cboTrangThai
            // 
            this.cboTrangThai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTrangThai.FormattingEnabled = true;
            this.cboTrangThai.Items.AddRange(new object[] {
            "Mới",
            "Đã duyệt",
            "Đã nhập kho",
            "Hủy"});
            this.cboTrangThai.Location = new System.Drawing.Point(112, 284);
            this.cboTrangThai.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboTrangThai.Name = "cboTrangThai";
            this.cboTrangThai.Size = new System.Drawing.Size(166, 21);
            this.cboTrangThai.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(15, 287);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 15);
            this.label11.TabIndex = 16;
            this.label11.Text = "Trạng thái:";
            // 
            // txtGhiChu
            // 
            this.txtGhiChu.Location = new System.Drawing.Point(112, 366);
            this.txtGhiChu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtGhiChu.Multiline = true;
            this.txtGhiChu.Name = "txtGhiChu";
            this.txtGhiChu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGhiChu.Size = new System.Drawing.Size(166, 58);
            this.txtGhiChu.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(15, 368);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 15);
            this.label10.TabIndex = 14;
            this.label10.Text = "Ghi chú:";
            // 
            // txtLyDoNhap
            // 
            this.txtLyDoNhap.Location = new System.Drawing.Point(112, 325);
            this.txtLyDoNhap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtLyDoNhap.Name = "txtLyDoNhap";
            this.txtLyDoNhap.Size = new System.Drawing.Size(166, 20);
            this.txtLyDoNhap.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 327);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 15);
            this.label9.TabIndex = 12;
            this.label9.Text = "Lý do nhập:";
            // 
            // txtNguoiGiao
            // 
            this.txtNguoiGiao.Location = new System.Drawing.Point(112, 244);
            this.txtNguoiGiao.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNguoiGiao.Name = "txtNguoiGiao";
            this.txtNguoiGiao.Size = new System.Drawing.Size(166, 20);
            this.txtNguoiGiao.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 246);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 15);
            this.label8.TabIndex = 10;
            this.label8.Text = "Người giao:";
            // 
            // txtKhoNhap
            // 
            this.txtKhoNhap.Location = new System.Drawing.Point(112, 203);
            this.txtKhoNhap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtKhoNhap.Name = "txtKhoNhap";
            this.txtKhoNhap.Size = new System.Drawing.Size(166, 20);
            this.txtKhoNhap.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 206);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 15);
            this.label7.TabIndex = 8;
            this.label7.Text = "Kho nhập:";
            // 
            // txtNhaCungCap
            // 
            this.txtNhaCungCap.Location = new System.Drawing.Point(112, 162);
            this.txtNhaCungCap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNhaCungCap.Name = "txtNhaCungCap";
            this.txtNhaCungCap.Size = new System.Drawing.Size(166, 20);
            this.txtNhaCungCap.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 165);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Nhà cung cấp:";
            // 
            // dtpNgayNhap
            // 
            this.dtpNgayNhap.CustomFormat = "dd/MM/yyyy";
            this.dtpNgayNhap.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgayNhap.Location = new System.Drawing.Point(112, 122);
            this.dtpNgayNhap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpNgayNhap.Name = "dtpNgayNhap";
            this.dtpNgayNhap.Size = new System.Drawing.Size(166, 20);
            this.dtpNgayNhap.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 124);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Ngày nhập:";
            // 
            // txtSoPhieu
            // 
            this.txtSoPhieu.Location = new System.Drawing.Point(112, 81);
            this.txtSoPhieu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSoPhieu.Name = "txtSoPhieu";
            this.txtSoPhieu.ReadOnly = true;
            this.txtSoPhieu.Size = new System.Drawing.Size(166, 20);
            this.txtSoPhieu.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Số phiếu:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "THÔNG TIN PHIẾU NHẬP";
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.LightGray;
            this.panelFooter.Controls.Add(this.btnHuy);
            this.panelFooter.Controls.Add(this.btnLuu);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 487);
            this.panelFooter.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelFooter.Size = new System.Drawing.Size(750, 57);
            this.panelFooter.TabIndex = 2;
            // 
            // btnHuy
            // 
            this.btnHuy.BackColor = System.Drawing.Color.Gray;
            this.btnHuy.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnHuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHuy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuy.ForeColor = System.Drawing.Color.White;
            this.btnHuy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHuy.Location = new System.Drawing.Point(592, 8);
            this.btnHuy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(75, 41);
            this.btnHuy.TabIndex = 1;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHuy.UseVisualStyleBackColor = false;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // btnLuu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.SeaGreen;
            this.btnLuu.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLuu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLuu.Location = new System.Drawing.Point(667, 8);
            this.btnLuu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(75, 41);
            this.btnLuu.TabIndex = 0;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLuu.UseVisualStyleBackColor = false;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // frmPhieuNhap_ChiTiet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 544);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimizeBox = false;
            this.Name = "frmPhieuNhap_ChiTiet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi Tiết Phiếu Nhập Kho";
            this.Load += new System.EventHandler(this.frmPhieuNhap_ChiTiet_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelChiTiet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            this.panelChiTietButtons.ResumeLayout(false);
            this.panelThongTin.ResumeLayout(false);
            this.panelThongTin.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelChiTiet;
        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.Panel panelChiTietButtons;
        private System.Windows.Forms.Button btnXoaChiTiet;
        private System.Windows.Forms.Button btnSuaChiTiet;
        private System.Windows.Forms.Button btnThemChiTiet;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.Panel panelThongTin;
        private System.Windows.Forms.ComboBox cboTrangThai;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtGhiChu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtLyDoNhap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtNguoiGiao;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtKhoNhap;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtNhaCungCap;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpNgayNhap;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSoPhieu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.Button btnLuu;
    }
}