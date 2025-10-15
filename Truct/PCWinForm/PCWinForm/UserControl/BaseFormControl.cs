using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCWinForm.UserControl
{
    public partial class BaseFormControl : Form
    {
        // Events và Properties giữ nguyên như trước
        public event EventHandler AddNewClicked;
        public event EventHandler EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler CopyClicked;
        public event EventHandler PrintClicked;
        public event EventHandler ImportExcelClicked;
        public event EventHandler ExportExcelClicked;
        public event EventHandler RefreshClicked;
        public event EventHandler SaveClicked;
        public event EventHandler CancelClicked;

        [Category("Data")]
        [Description("Tiêu đề form")]
        public string FormTitle
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        [Browsable(false)]
        public DataGridView DataGridView => dataGridView;

        [Browsable(false)]
        public Panel EditPanel => pnlEditContent;

        [Browsable(false)]
        public bool IsEditMode => pnlEdit.Visible;

        public BaseFormControl()
        {
            InitializeComponent();
            SetupPermissions();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
        }

        private void SetupPermissions()
        {
            // TODO: Load permissions from user role
        }

        #region Button Click Handlers
        private void btnAdd_Click(object sender, EventArgs e) => AddNewClicked?.Invoke(this, e);
        private void btnEdit_Click(object sender, EventArgs e) => EditClicked?.Invoke(this, e);
        private void btnDelete_Click(object sender, EventArgs e) => DeleteClicked?.Invoke(this, e);
        private void btnCopy_Click(object sender, EventArgs e) => CopyClicked?.Invoke(this, e);
        private void btnPrint_Click(object sender, EventArgs e) => PrintClicked?.Invoke(this, e);
        private void btnImportExcel_Click(object sender, EventArgs e) => ImportExcelClicked?.Invoke(this, e);
        private void btnExportExcel_Click(object sender, EventArgs e) => ExportExcelClicked?.Invoke(this, e);
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshClicked?.Invoke(this, e);
        private void btnSave_Click(object sender, EventArgs e) => SaveClicked?.Invoke(this, e);
        private void btnCancel_Click(object sender, EventArgs e) => CancelClicked?.Invoke(this, e);
        #endregion

        #region Public Methods
        public void SetButtonPermissions(bool canAdd, bool canEdit, bool canDelete,
                                       bool canCopy = true, bool canPrint = true,
                                       bool canExport = true, bool canImport = true)
        {
            btnAdd.Enabled = canAdd;
            btnEdit.Enabled = canEdit;
            btnDelete.Enabled = canDelete;
            btnCopy.Enabled = canCopy;
            btnPrint.Enabled = canPrint;
            btnExportExcel.Enabled = canExport;
            btnImportExcel.Enabled = canImport;
        }

        public void SetEditMode(bool isEditMode)
        {
            pnlGrid.Visible = !isEditMode;
            pnlEdit.Visible = isEditMode;

            if (isEditMode)
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        public void ShowMessage(string message, string title = "Thông báo",
                              MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        public DialogResult ShowConfirm(string message, string title = "Xác nhận")
        {
            return MessageBox.Show(message, title,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
        }

        public void AddEditControl(Control control)
        {
            pnlEditContent.Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }

        public void ClearEditControls()
        {
            pnlEditContent.Controls.Clear();
        }
        #endregion
#endregion
    }
}
