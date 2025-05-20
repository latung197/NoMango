//==================================================================================================
// System  : DenKa
// File    : MenuForm.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// PC001_Màn hình Menu
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using PlastMB.Helper;
using PlastMB.Model;
using PlastMB.Properties;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Windows.Forms;

namespace PlastMB
{
    public partial class MenuForm : BaseForm
    {
        #region - Definition -

        private FileHelper watcher;

        private string folderPathSaveCSV;
        private string folderPathSaveExcel;

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public MenuForm()
        {
            InitializeComponent();
        }

        #endregion

        #region - Method -

        /// <summary>
        /// Executed when the UI form loads.
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);

                try
                {
                    var savedFolderPaths = JsonConvert.DeserializeObject<FolderPaths>(json);

                    // Check if savedFolderPaths is not null
                    if (savedFolderPaths != null)
                    {

                    }
                }
                catch (Exception ex)
                {
                    //var message = ResourceLoader.GetForViewIndependentUse().GetString("ErrorConfig");
                    var title = "Có lỗi xảy ra!";
                    MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    logger.Error(Utility.GetExceptionInfo(ex, "MenuForm.cs"));
                }
            }
            else
            {
                // Create the message dialog and set its content
                var title = "Không tìm thấy tập tin cấu hình";
                MessageBox.Show(filePath, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Initialize HA folder watcher
        /// </summary>
        /// <param name="folderPath"></param>
        private void WatchFolder(string folderPath)
        {
            //instantiate the watcher
            watcher = new FileHelper(folderPath);
            watcher.RunWatcher();

            //Subscribe to the new CSV file event
            watcher.FileEvent += WatcherCSV_FileEvent;

            //Run for the first time
            var newfile = watcher.ScanFolder();
            UpdateCSVAsync(newfile.Length);
        }

        /// <summary>
        /// New CSV file event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatcherCSV_FileEvent(object sender, EventArgs e)
        {
            try
            {
                if (e == null) return;
                var evet = (MsgFileArgs<FileInfo>)e;
                UpdateCSVAsync(evet.Records.Count);
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "MenuForm.cs"));
            }
        }

        /// <summary>
        /// Update CSV CSV count
        /// </summary>
        /// <param name="newfile"></param>
        private void UpdateCSVAsync(int newfile)
        {
            menuButton2.ShowNumber(newfile);
        }

        /// <summary>
        /// Show No HA CSV config dialog
        /// </summary>
        private void ShowDialogNoHAConfig()
        {
            var message = "Đường dẫn file CSV sinh ra từ hệ thống";
            var title = "Không tìm thấy cấu hình";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Stop folder watcher
        /// </summary>
        private void StopWatcher()
        {
            watcher?.Stop();
        }

        #endregion

        #region - Event -

        private void titleBar1_MouseDowned(object sender, EventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void titleBar1_CloseClicked(object sender, System.EventArgs e)
        {
            Close();
        }

        private void MenuForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                // "appsetting.json"
                var filePath = Settings.Default.FileConfig;
                LoadFromJson(filePath);
            }
            else
            {
                StopWatcher();
            }
        }

        private void menuButton1_Click(object sender, EventArgs e)
        {
            
            var settingForm = new SettingForm();
            settingForm.Show();
        }

        private void menuButton2_Click(object sender, EventArgs e)
        {
            var haForm = new ImportHistoryForm();
            haForm.Show();
        }

        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopWatcher();
        }

        #endregion

    }
}