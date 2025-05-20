using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class TransparentPanel : Panel
    {
        public TransparentPanel()
        {
            //base.InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Calling the base class OnPaint
            //base.OnPaint(e);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(75, 0, 0, 0)), this.ClientRectangle);
        }
    }
}
