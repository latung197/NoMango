//==================================================================================================
// System  : DenKa
// File    : SettingForm.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// PC002_Màn hình thiết lập đường dẫn
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using PlastMB.Common;
using PlastMB.Model;
using PlastMB.Properties;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Windows.Forms;

namespace PlastMB
{
    public partial class SettingForm : BaseForm
    {
        #region - Definition -

        //public event EventHandler CloseClicked;   //Default event

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public SettingForm()
        {
            InitializeComponent();

            Form1_Load();
        }

        // Khi ứng dụng được khởi động
        private void Form1_Load()
        {
            textBox1.Click += Form1_Click;
            faButton2.Enabled = true;
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsetting.json");
            var filePath = Settings.Default.FileConfig;

            // Kiểm tra xem thư mục có tồn tại hay không
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show(directoryPath, "Không tìm thấy thư mục.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                faButton2.Enabled = false;
                return;
            }

            if (!File.Exists(filePath))
            {
                faButton2.Enabled = false;
                MessageBox.Show("Không tìm thấy file config.");
                return;
            }
            else
            {
                LoadFromJson(filePath);
            }
        }

        #endregion

        #region - Method -

        private void Form1_Click(object sender, EventArgs e)
        {
            TextBox clickedTextBox = sender as TextBox;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                clickedTextBox.Text = folderBrowserDialog.SelectedPath;
                clickedTextBox.TextAlign = HorizontalAlignment.Left;
            }
        }

        public void SaveToJson(string filePath, FolderPaths folderPaths)
        {
            string json = JsonConvert.SerializeObject(folderPaths);
            File.WriteAllText(filePath, json);
        }

        // Đọc dữ liệu từ tệp JSON và điền vào các TextBox
        public void LoadFromJson(string filePath)
        {

            string json = File.ReadAllText(filePath);
            FolderPaths savedFolderPaths = JsonConvert.DeserializeObject<FolderPaths>(json);

            // Gán các giá trị từ tệp JSON vào các TextBox
            textBox1.Text = savedFolderPaths.Folder1;
            nrd_TimeReadCsv.Value = int.Parse(savedFolderPaths.TimeReadCsv);


            // Kiểm tra và thiết lập TextAlign cho các TextBox
            SetTextBoxTextAlign(textBox1);
        }
        private void SetTextBoxTextAlign(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text != "。。。")
            {
                textBox.TextAlign = HorizontalAlignment.Left;
            }
        }

        #endregion

        #region - Event -

        private void titleBar1_MouseDowned(object sender, System.EventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void titleBar1_CloseClicked(object sender, System.EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) && textBox1.Text == "。。。")
                {
                    textBox1.BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    FolderPaths folderPaths = new FolderPaths
                    {
                        Folder1 = textBox1.Text,
                        TimeReadCsv = nrd_TimeReadCsv.Value.ToString()
                    };
                    //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsetting.json");
                    var filePath = Settings.Default.FileConfig;

                    SaveToJson(filePath, folderPaths);

                    DialogResult result = MessageBox.Show("設定情報を保存しました。\nメニュー画面に戻りますか？", "成功", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.OK)
                    {
                        Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(Utility.GetExceptionInfo(ex, "SettingForm.cs"));
            }
        }

        private void SettingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utility.ShowForm(Constant.MenuTitle);
        }

        #endregion
    }
}
