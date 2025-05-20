using PlastMB.Helper;
using PlastMB.Model;
using NLog;
using System;
using System.IO;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
    public partial class DeleteForm : Form
    {
        #region - Definition -

        //public event EventHandler DataLoaded;

        //private readonly FileInfo file;
        //private readonly CSVInfo data;

        //private string fileName;
        //private string extension;

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        //public DeleteForm(FileInfo file, CSVInfo data)
        public DeleteForm()
        {
            InitializeComponent();

            //this.file = file;
            //this.data = data;

            //fileName = Path.GetFileNameWithoutExtension(file.Name);
            //extension = Path.GetExtension(file.Name);

            //this.textBox1.Text = fileName;
        }

        #endregion

        #region - Method -


        #endregion

        #region - Event -

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void faButton1_Click(object sender, EventArgs e)
        {
            try
            {
                var pass = textBox1.Text;

                if (string.IsNullOrEmpty(pass))
                {
                    label1.Text = "Mật khẩu không được để trống.";
                    label1.Visible = true;
                    return;
                }

                if (pass != "vinitape")
                {
                    label1.Text = "Mật khẩu không đúng.";
                    label1.Visible = true;
                    return; 
                }

                /*var isFileLocked = TestResultHelper.IsFileLocked(file.FullName);
                if (isFileLocked)
                {   // Tệp hiện đang được mở bởi một ứng dụng khác.
                    var title = "Có lỗi xảy ra!";
                    var message = "Không thể đổi tên do tệp CSV đang được mở bởi một ứng dụng khác.";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }*/

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Có lỗi xảy ra!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void faButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

    }
}
