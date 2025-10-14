namespace AIOI.View
{
    partial class frmChonVatTu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChonVatTu));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblSoLuongChon = new System.Windows.Forms.Label();
            this.lblKetQua = new System.Windows.Forms.Label();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.dgvVatTu = new System.Windows.Forms.DataGridView();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblNhaCungCap = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblNhom = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblDonGia = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDonViTinh = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTenVatTu = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMaVatTu = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnBoChonTatCa = new System.Windows.Forms.Button();
            this.btnChonTatCa = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnChon = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVatTu)).BeginInit();
            this.panelInfo.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.SteelBlue;
            this.panelHeader.Controls.Add(this.label1);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(900, 60);
            this.panelHeader.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "CHỌN VẬT TƯ";
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelSearch.Controls.Add(this.lblSoLuongChon);
            this.panelSearch.Controls.Add(this.lblKetQua);
            this.panelSearch.Controls.Add(this.txtTimKiem);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 60);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(10);
            this.panelSearch.Size = new System.Drawing.Size(900, 60);
            this.panelSearch.TabIndex = 1;
            // 
            // lblSoLuongChon
            // 
            this.lblSoLuongChon.AutoSize = true;
            this.lblSoLuongChon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoLuongChon.ForeColor = System.Drawing.Color.Green;
            this.lblSoLuongChon.Location = new System.Drawing.Point(600, 23);
            this.lblSoLuongChon.Name = "lblSoLuongChon";
            this.lblSoLuongChon.Size = new System.Drawing.Size(133, 18);
            this.lblSoLuongChon.TabIndex = 3;
            this.lblSoLuongChon.Text = "Đã chọn: 0 vật tư";
            // 
            // lblKetQua
            // 
            this.lblKetQua.AutoSize = true;
            this.lblKetQua.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKetQua.Location = new System.Drawing.Point(350, 23);
            this.lblKetQua.Name = "lblKetQua";
            this.lblKetQua.Size = new System.Drawing.Size(120, 18);
            this.lblKetQua.TabIndex = 2;
            this.lblKetQua.Text = "Tìm thấy: 0 vật tư";
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Location = new System.Drawing.Point(100, 20);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(200, 22);
            this.txtTimKiem.TabIndex = 1;
            this.txtTimKiem.TextChanged += new System.EventHandler(this.txtTimKiem_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tìm kiếm:";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.dgvVatTu);
            this.panelMain.Controls.Add(this.panelInfo);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 120);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(900, 400);
            this.panelMain.TabIndex = 2;
            // 
            // dgvVatTu
            // 
            this.dgvVatTu.AllowUserToAddRows = false;
            this.dgvVatTu.AllowUserToDeleteRows = false;
            this.dgvVatTu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVatTu.BackgroundColor = System.Drawing.Color.White;
            this.dgvVatTu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVatTu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVatTu.Location = new System.Drawing.Point(0, 0);
            this.dgvVatTu.Name = "dgvVatTu";
            this.dgvVatTu.RowHeadersWidth = 51;
            this.dgvVatTu.RowTemplate.Height = 24;
            this.dgvVatTu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVatTu.Size = new System.Drawing.Size(600, 400);
            this.dgvVatTu.TabIndex = 1;
            this.dgvVatTu.SelectionChanged += new System.EventHandler(this.dgvVatTu_SelectionChanged);
            this.dgvVatTu.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVatTu_CellDoubleClick);
            this.dgvVatTu.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVatTu_CellContentClick);
            this.dgvVatTu.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVatTu_CellValueChanged);
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelInfo.Controls.Add(this.lblNhaCungCap);
            this.panelInfo.Controls.Add(this.label9);
            this.panelInfo.Controls.Add(this.lblNhom);
            this.panelInfo.Controls.Add(this.label7);
            this.panelInfo.Controls.Add(this.lblDonGia);
            this.panelInfo.Controls.Add(this.label5);
            this.panelInfo.Controls.Add(this.lblDonViTinh);
            this.panelInfo.Controls.Add(this.label6);
            this.panelInfo.Controls.Add(this.lblTenVatTu);
            this.panelInfo.Controls.Add(this.label4);
            this.panelInfo.Controls.Add(this.lblMaVatTu);
            this.panelInfo.Controls.Add(this.label3);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelInfo.Location = new System.Drawing.Point(600, 0);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Padding = new System.Windows.Forms.Padding(15);
            this.panelInfo.Size = new System.Drawing.Size(300, 400);
            this.panelInfo.TabIndex = 0;
            // 
            // lblNhaCungCap
            // 
            this.lblNhaCungCap.BackColor = System.Drawing.Color.White;
            this.lblNhaCungCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNhaCungCap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNhaCungCap.Location = new System.Drawing.Point(18, 320);
            this.lblNhaCungCap.Name = "lblNhaCungCap";
            this.lblNhaCungCap.Size = new System.Drawing.Size(264, 30);
            this.lblNhaCungCap.TabIndex = 11;
            this.lblNhaCungCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 300);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(124, 18);
            this.label9.TabIndex = 10;
            this.label9.Text = "Nhà cung cấp:";
            // 
            // lblNhom
            // 
            this.lblNhom.BackColor = System.Drawing.Color.White;
            this.lblNhom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNhom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNhom.Location = new System.Drawing.Point(18, 250);
            this.lblNhom.Name = "lblNhom";
            this.lblNhom.Size = new System.Drawing.Size(264, 30);
            this.lblNhom.TabIndex = 9;
            this.lblNhom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 18);
            this.label7.TabIndex = 8;
            this.label7.Text = "Nhóm:";
            // 
            // lblDonGia
            // 
            this.lblDonGia.BackColor = System.Drawing.Color.White;
            this.lblDonGia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDonGia.ForeColor = System.Drawing.Color.Red;
            this.lblDonGia.Location = new System.Drawing.Point(18, 180);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(264, 30);
            this.lblDonGia.TabIndex = 7;
            this.lblDonGia.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 160);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "Đơn giá:";
            // 
            // lblDonViTinh
            // 
            this.lblDonViTinh.BackColor = System.Drawing.Color.White;
            this.lblDonViTinh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDonViTinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDonViTinh.Location = new System.Drawing.Point(18, 110);
            this.lblDonViTinh.Name = "lblDonViTinh";
            this.lblDonViTinh.Size = new System.Drawing.Size(264, 30);
            this.lblDonViTinh.TabIndex = 5;
            this.lblDonViTinh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 18);
            this.label6.TabIndex = 4;
            this.label6.Text = "Đơn vị tính:";
            // 
            // lblTenVatTu
            // 
            this.lblTenVatTu.BackColor = System.Drawing.Color.White;
            this.lblTenVatTu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTenVatTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenVatTu.Location = new System.Drawing.Point(18, 60);
            this.lblTenVatTu.Name = "lblTenVatTu";
            this.lblTenVatTu.Size = new System.Drawing.Size(264, 30);
            this.lblTenVatTu.TabIndex = 3;
            this.lblTenVatTu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "Tên vật tư:";
            // 
            // lblMaVatTu
            // 
            this.lblMaVatTu.BackColor = System.Drawing.Color.White;
            this.lblMaVatTu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMaVatTu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaVatTu.ForeColor = System.Drawing.Color.Blue;
            this.lblMaVatTu.Location = new System.Drawing.Point(18, 10);
            this.lblMaVatTu.Name = "lblMaVatTu";
            this.lblMaVatTu.Size = new System.Drawing.Size(264, 30);
            this.lblMaVatTu.TabIndex = 1;
            this.lblMaVatTu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, -10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Mã vật tư:";
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.LightGray;
            this.panelFooter.Controls.Add(this.btnBoChonTatCa);
            this.panelFooter.Controls.Add(this.btnChonTatCa);
            this.panelFooter.Controls.Add(this.btnHuy);
            this.panelFooter.Controls.Add(this.btnChon);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 520);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Padding = new System.Windows.Forms.Padding(10);
            this.panelFooter.Size = new System.Drawing.Size(900, 70);
            this.panelFooter.TabIndex = 3;
            // 
            // btnBoChonTatCa
            // 
            this.btnBoChonTatCa.BackColor = System.Drawing.Color.Gray;
            this.btnBoChonTatCa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBoChonTatCa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBoChonTatCa.ForeColor = System.Drawing.Color.White;
            this.btnBoChonTatCa.Location = new System.Drawing.Point(120, 15);
            this.btnBoChonTatCa.Name = "btnBoChonTatCa";
            this.btnBoChonTatCa.Size = new System.Drawing.Size(100, 40);
            this.btnBoChonTatCa.TabIndex = 3;
            this.btnBoChonTatCa.Text = "Bỏ chọn tất cả";
            this.btnBoChonTatCa.UseVisualStyleBackColor = false;
            this.btnBoChonTatCa.Click += new System.EventHandler(this.btnBoChonTatCa_Click);
            // 
            // btnChonTatCa
            // 
            this.btnChonTatCa.BackColor = System.Drawing.Color.SteelBlue;
            this.btnChonTatCa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChonTatCa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChonTatCa.ForeColor = System.Drawing.Color.White;
            this.btnChonTatCa.Location = new System.Drawing.Point(10, 15);
            this.btnChonTatCa.Name = "btnChonTatCa";
            this.btnChonTatCa.Size = new System.Drawing.Size(100, 40);
            this.btnChonTatCa.TabIndex = 2;
            this.btnChonTatCa.Text = "Chọn tất cả";
            this.btnChonTatCa.UseVisualStyleBackColor = false;
            this.btnChonTatCa.Click += new System.EventHandler(this.btnChonTatCa_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.BackColor = System.Drawing.Color.Gray;
            this.btnHuy.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnHuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHuy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuy.ForeColor = System.Drawing.Color.White;
            //this.btnHuy.Image = ((System.Drawing.Image)(resources.GetObject("btnHuy.Image")));
            this.btnHuy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHuy.Location = new System.Drawing.Point(690, 10);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(100, 50);
            this.btnHuy.TabIndex = 1;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHuy.UseVisualStyleBackColor = false;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // btnChon
            // 
            this.btnChon.BackColor = System.Drawing.Color.SeaGreen;
            this.btnChon.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnChon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChon.ForeColor = System.Drawing.Color.White;
            //this.btnChon.Image = ((System.Drawing.Image)(resources.GetObject("btnChon.Image")));
            this.btnChon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChon.Location = new System.Drawing.Point(790, 10);
            this.btnChon.Name = "btnChon";
            this.btnChon.Size = new System.Drawing.Size(100, 50);
            this.btnChon.TabIndex = 0;
            this.btnChon.Text = "Chọn";
            this.btnChon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChon.UseVisualStyleBackColor = false;
            this.btnChon.Click += new System.EventHandler(this.btnChon_Click);
            // 
            // frmChonVatTu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 590);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.panelHeader);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChonVatTu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn Vật Tư";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVatTu)).EndInit();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblSoLuongChon;
        private System.Windows.Forms.Label lblKetQua;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.DataGridView dgvVatTu;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblNhaCungCap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblNhom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblDonGia;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDonViTinh;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTenVatTu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMaVatTu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.Button btnChon;
        private System.Windows.Forms.Button btnBoChonTatCa;
        private System.Windows.Forms.Button btnChonTatCa;
    }
}