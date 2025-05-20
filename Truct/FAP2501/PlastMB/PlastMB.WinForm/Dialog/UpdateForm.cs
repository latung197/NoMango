using PlastMB.Common;
using PlastMB.Helper;
using PlastMB.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
    public partial class UpdateForm : Form
    {
        #region - Definition -

        public event EventHandler DataLoaded;

        private readonly FileInfo file;
        private readonly CSVInfo data;

        private readonly BindingList<Data> source = new BindingList<Data>();

        private List<string> values;
        private List<string> forces;
        public int column;
        public int row;
        public bool fillAuto = false;

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public UpdateForm(FileInfo file, CSVInfo data)
        {
            InitializeComponent();

            this.file = file;
            this.data = data;
            column = -1;
            row = -1;
        }

        #endregion

        #region - Method -

        private void LoadData(FileInfo file, CSVInfo data)
        {
            try
            {
                var filePath = TestResultHelper.GetExcelPath(Utility.FOLDER_EXCEL, file);
                if (string.IsNullOrEmpty(filePath))
                {
                    var title = "Tệp csv có tên không đúng định dạng.";
                    MessageBox.Show(file.FullName, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }
                if (!File.Exists(filePath))
                {
                    filePath = TestResultHelper.GetExcelPath(Utility.FOLDER_EXCEL, file, ".xlsm");
                    if (!File.Exists(filePath))
                    {
                        var title = "Không tìm thấy tệp Excel";
                        MessageBox.Show(filePath, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                        return;
                    }
                }
                var isFileLocked = TestResultHelper.IsFileLocked(filePath);
                if (isFileLocked)
                {
                    var title = "Tệp hiện đang được mở bởi một ứng dụng khác.";
                    MessageBox.Show(filePath, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                forces = new List<string>();
                //var pw = TestResultHelper.GetExcelPw(file.Name);
                var pw = string.Empty;
                values = TestResultInterop.GetExcelCellsAsync(filePath, pw, file, data, ref forces, ref column, ref row, ref fillAuto);
                GC.Collect();

                if (column < 0)
                {
                    var title = "Update";
                    var message = "Hệ thống không tìm thấy giá trị tương ứng để cập nhật.";
                    if (column == (int)ExportState.PassIncorrect)
                    {
                        message = "Password không đúng.";
                    }
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                var index = 0;
                var basename = Path.GetFileNameWithoutExtension(file.Name);
                basename = basename.ToUpper();

                if (!fillAuto)
                {
                    label1.Visible = true;
                }

                foreach (var force in forces)
                {
                    var value = 0.0d;
                    if (index < row || row <= 0)
                    {
                        if (forces.Count == 1)
                        {
                            if (basename.Contains(Constant.TEST_RESULT_260W))
                            {
                                value = data.GetForceAverageN19mm();
                            }
                            else
                            {
                                value = data.GetForceAverage();
                            }
                        }
                        else
                        {
                            if (basename.Contains(Constant.TEST_RESULT_260W))
                            {
                                value = data.GetForceResultN19mm(index);
                            }
                            else
                            {
                                value = data.GetForceResult(index);
                            }
                        }
                    }
                    else
                    {
                        value = data.GetElongationResult(index - row);
                    }
                    var oldvalue = string.Empty;
                    if (index < values.Count)
                    {
                        oldvalue = values[index];
                    }
                    var dt = new Data
                    {
                        Force = force,
                        Value = oldvalue,
                        Update = value > 0 ? value.ToString() : string.Empty,
                    };
                    source.Add(dt);
                    index++;
                }
                dataGridView1.DataSource = source;
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "UpdateForm.cs"));
            }
        }

        #endregion

        #region - Event -

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            LoadData(file, data);
            //DataLoaded?.Invoke(sender, e);
            DataLoaded?.Invoke(sender, e);
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
            DialogResult = DialogResult.OK;
        }

        private void faButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

    }
}
