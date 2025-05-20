using PlastMB.Extension;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class InfoBadge : Label
    {
        #region - Definition -

        private readonly int _padding = 8;
        private readonly int _radius = 50;
        private readonly SolidBrush _backgroundBrush = new SolidBrush(Color.FromArgb(196, 43, 28));

        private readonly Pen _borderPen = new Pen(ControlPaint.Light(Color.Black, 0.0f), 4.0f);

        #endregion

        #region - Initialize -

        public InfoBadge()
        {
            this.ForeColor = Color.White;
            this.Font = new Font("Arial", 12F, FontStyle.Regular);
            //this.Padding = new Padding(17, 11, 0, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            drawBorder(g);
            drawBackground(g);

            base.OnPaint(e);
        }

        private void drawBorder(Graphics g) => g.DrawRoundedRectangle(_borderPen, _padding, _padding, Width - (_padding * 2), Height - (_padding * 2), _radius);

        private void drawBackground(Graphics g) => g.FillRoundedRectangle(_backgroundBrush, _padding, _padding, Width - (_padding * 2), Height - (_padding * 2), _radius);

        #endregion

    }
}
