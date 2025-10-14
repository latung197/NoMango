using System.Drawing;
using System.Windows.Forms;

namespace AIOI
{
    partial class FormMain
    {
        public FormCustomer()
        {
            InitializeComponent();
            InitializeCustomerForm();
        }

        private void InitializeCustomerForm()
        {
            this.Text = "Quản lý Khách hàng";
            this.Size = new System.Drawing.Size(800, 600);

            // Thêm các control quản lý khách hàng ở đây
            Label lblTitle = new Label();
            lblTitle.Text = "QUẢN LÝ KHÁCH HÀNG";
            lblTitle.Font = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            // Thêm DataGridView, buttons, etc.
            DataGridView grid = new DataGridView();
            grid.Location = new System.Drawing.Point(20, 60);
            grid.Size = new System.Drawing.Size(740, 400);
            this.Controls.Add(grid);
        }
    }
}