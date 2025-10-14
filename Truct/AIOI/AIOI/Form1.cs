using AioiSystems.Lightstep;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIOI
{
    public partial class Form1 : Form
    {
        private EthernetController _controller;
        private readonly object _lockObject = new object();

        public Form1()
        {
            InitializeComponent();
            InitializeController();
        }

        private void InitializeController()
        {
            _controller = new EthernetController();

            // Đăng ký sự kiện
            _controller.CommandReceived += Controller_CommandReceived;
            _controller.ErrorRaised += Controller_ErrorRaised;
            _controller.EndConnect += Controller_EndConnect;

            // Đồng bộ với UI thread
            _controller.SynchronizingObject = this;
        }

        void _controller_CommandReceived(object sender, EventArgs e)
        {
            CommandInfo command = _controller.GetCommand();
            if (command != null)
            {
                Console.WriteLine(command.GetCommandText());
            }
        }

        private void Controller_CommandReceived(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                CommandInfo command = _controller.GetCommand();
                if (command != null)
                {
                    string commandText = command.GetCommandText();

                    // Hiển thị lệnh nhận được
                    AddLog($"Nhận lệnh: {commandText}");

                    // Xử lý lệnh "t" từ nút bấm
                    if (commandText.StartsWith("t"))
                    {
                        ProcessButtonCommand(command);
                    }
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị trạng thái kết nối
                lblStatus.Text = "Đang kết nối...";
                lblStatus.ForeColor = System.Drawing.Color.Orange;

                // Thiết lập license (thay bằng license thật)
                _controller.SetLicense("71D1-AF23-0E9E-10FD-1829");

                // Kết nối bất đồng bộ
                _controller.BeginConnect(txtIP.Text, int.Parse(txtPort.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Controller_EndConnect(object sender, EndConnectEventArgs e)
        {
            if (e.Error != null)
            {
                lblStatus.Text = "Kết nối thất bại";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Lỗi kết nối: {e.Error.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                lblStatus.Text = "Đã kết nối";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                AddLog("Kết nối thành công đến controller");
            }
        }



        private void ProcessButtonCommand(CommandInfo command)
        {
            try
            {
                AddressInfo[] addresses = AddressInfo.SplitCommand(command.GetCommandBytes());

                foreach (AddressInfo address in addresses)
                {
                    string logMessage = $"Địa chỉ: {address.Address}, Trạng thái: {address.Status}";
                    AddLog(logMessage);

                    // Hiển thị lên ListView
                    UpdateAddressListView(address);

                    // Xử lý theo trạng thái nút
                    switch (address.Status)
                    {
                        case "1": // Nút được nhấn
                            HandleButtonPress(address.Address);
                            break;
                        case "0": // Nút được nhả
                            HandleButtonRelease(address.Address);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"Lỗi xử lý lệnh: {ex.Message}");
            }
        }

        private void HandleButtonPress(string address)
        {
            // Xử lý khi nút được nhấn
            AddLog($"→ Nút {address} được NHẤN");

            // Cập nhật UI
            if (InvokeRequired)
            {
                Invoke(new Action<string>(HandleButtonPress), address);
                return;
            }

            // Hiển thị thông báo
            lblLastAction.Text = $"Nút {address} được nhấn";
            lblLastAction.ForeColor = System.Drawing.Color.Red;

            // Thêm vào listbox
            //lstEvents.Items.Add($"{DateTime.Now:HH:mm:ss} - Nút {address} NHẤN");
            //lstEvents.SelectedIndex = lstEvents.Items.Count - 1;
        }

        private void HandleButtonRelease(string address)
        {
            // Xử lý khi nút được nhả
            AddLog($"→ Nút {address} được NHẢ");

            if (InvokeRequired)
            {
                Invoke(new Action<string>(HandleButtonRelease), address);
                return;
            }

            lblLastAction.Text = $"Nút {address} được nhả";
            lblLastAction.ForeColor = System.Drawing.Color.Blue;
        }

        private void UpdateAddressListView(AddressInfo address)
        {
            if (lstAddresses.InvokeRequired)
            {
                lstAddresses.Invoke(new Action<AddressInfo>(UpdateAddressListView), address);
                return;
            }

            // Tìm item có cùng địa chỉ
            ListViewItem item = null;
            foreach (ListViewItem existingItem in lstAddresses.Items)
            {
                if (existingItem.Text == address.Address)
                {
                    item = existingItem;
                    break;
                }
            }

            if (item == null)
            {
                // Thêm mới
                item = new ListViewItem(address.Address);
                item.SubItems.Add(address.Status);
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
                lstAddresses.Items.Add(item);
            }
            else
            {
                // Cập nhật
                item.SubItems[1].Text = address.Status;
                item.SubItems[2].Text = DateTime.Now.ToString("HH:mm:ss");
            }
        }

        private void Controller_ErrorRaised(object sender, ErrorRaisedEventArgs e)
        {
            AddLog($"LỖI: {e.Error.Message}");
        }

        private void AddLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(AddLog), message);
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.ScrollToCaret();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.Close();
                lblStatus.Text = "Ngắt kết nối";
                lblStatus.ForeColor = System.Drawing.Color.Gray;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                AddLog("Đã ngắt kết nối");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ngắt kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            lstAddresses.Items.Clear();
        }

        private void btnTestCommand_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.IsConnected)
                {
                    CommandInfo response = _controller.SendCommand("Z");
                    AddLog($"Test response: {response.GetCommandText()}");
                }
                else
                {
                    MessageBox.Show("Chưa kết nối đến controller", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                AddLog($"Lỗi test: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Đảm bảo đóng kết nối khi thoát
            if (_controller != null && _controller.IsConnected)
            {
                _controller.Close();
            }
        }

        private void lstEvents_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSent_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.IsConnected)
                {
                    string cmd = txtCmd.Text;
                    CommandInfo response = _controller.SendCommand(cmd);
                    AddLog($"Test response: {response.GetCommandText()}");
                }
                else
                {
                    MessageBox.Show("Chưa kết nối đến controller", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                AddLog($"Lỗi test: {ex.Message}");
            }
        }
    }
}





