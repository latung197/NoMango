using PlastMB.Model;
//using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
    public partial class UpgradeForm : Form
    {
        #region - Definition -

        //private readonly List<Data> source = new List<Data>();
        //public int row;
        public int column;
        public bool fillAuto = false;

        private bool needRefresh = true;

        // create a static logger field
        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public UpgradeForm(List<Data> source, int column, bool fillAuto)
        {
            InitializeComponent();

            //this.source = source;
            dataGridView1.DataSource = source;

            this.column = column;
            this.fillAuto = fillAuto;
        }

        #endregion

        #region - Method -

        private void LoadData()
        {
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.FromArgb(146, 209, 79);
            dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.Yellow;
        }

        #endregion

        #region - Event -

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void faButton1_Click(object sender, EventArgs e)
        {
            if (column < 0)
            {
                //var title = dateTimePickerBorder1.Date.ToString("d/MM/yyyy");
                var message = "Hệ thống không tìm thấy giá trị tương ứng để cập nhật.";
                MessageBox.Show(message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                return;
            }
            if (!fillAuto)
            {
                //var key = $"{data.LotNo} {data.Color} {(data.NL ? "NL " : "")}";
                var title = "Dữ liệu không được tạo bằng hệ thống";
                var message = $"Dữ liệu cũ không được tạo bằng hệ thống.\nBạn có chắc chắn muốn cập nhật liệu không?";
                var warning = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (warning == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                    return;
                }
                else if (warning == DialogResult.No)
                {
                    //DialogResult = DialogResult.Cancel;
                    return;
                }
                else if (warning == DialogResult.Cancel)
                {
                    return;
                }
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật vào dữ liệu đo cũ không?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
                DialogResult = DialogResult.OK;
            //else
            //    DialogResult = DialogResult.Cancel;
        }

        private void faButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /*private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == 1)
            {
                dataGridView1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.FromArgb(146, 209, 79);
            }
            else if (e.ColumnIndex == 2)
            {
                dataGridView1.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.Yellow;
            }
            //logger.Info($"CellPainting: {e.RowIndex} x {e.ColumnIndex}");
        }*/

        /*private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //logger.Info($"Data Binding Complete: {e}");
            dataGridView1.Invalidate();
        }*/

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            if (needRefresh)
            {
                needRefresh = false;
                dataGridView1.Invalidate();
            }
            //logger.Info($"Paint: {needRefresh} {e}");
        }

        #endregion
    }
}
