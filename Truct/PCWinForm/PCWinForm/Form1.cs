using System.Runtime.InteropServices;

namespace PCWinForm
{
    public partial class Form1 : Form
    {
        NotifyIcon trayIcon;
        private bool _allowExit = false;
        private const int HOTKEY_ID = 100;
        private const int MOD_ALT = 0x0001;
        private const int WM_HOTKEY = 0x0312;

        private bool _isListening = false;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "Tool chạy ngầm",
                Visible = true
            };

            trayIcon.DoubleClick += (s, e2) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Exit", null, (s, e) =>
            {
                _allowExit = true;
                trayIcon.Visible = false;
                Application.Exit();
            });
            trayIcon.ContextMenuStrip = menu;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowExit)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_isListening) return;

            bool ok = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_ALT, (int)Keys.Q);
            if (!ok)
            {
                MessageBox.Show("Alt + Q đang bị app khác sử dụng");
                return;
            }

            _isListening = true;
            lblStatus.Text = "Đang nhận Alt + Q";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_isListening) return;

            UnregisterHotKey(this.Handle, HOTKEY_ID);
            _isListening = false;

            lblStatus.Text = "Đã dừng nhận Alt + Q";
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                if (!_isListening) return;

                Point pos = Cursor.Position;

                Clipboard.SetText($"X={pos.X}, Y={pos.Y}");
                // hoặc lưu file, log, vẽ UI...
            }

            base.WndProc(ref m);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_isListening)
            {
                UnregisterHotKey(this.Handle, HOTKEY_ID);
            }

            base.OnFormClosed(e);
        }
    }
}
