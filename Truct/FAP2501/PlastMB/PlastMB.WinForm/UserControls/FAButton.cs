using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlastMB.Extension;

namespace PlastMB.UserControls
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class FAButton : Button
    {
        #region - Definition -

        private readonly int _padding = 2;
        private readonly int _radius = 4;

        public Color ColorTop
        {
            get; set;
        } = Color.FromArgb(255, 235, 218);
        public Color ColorBottom
        {
            get; set;
        } = Color.FromArgb(255, 190, 135);

        private readonly SolidBrush _backgroundBrush = new SolidBrush(Color.FromArgb(196, 43, 28));

        private readonly Pen _borderPen = new Pen(ControlPaint.Light(Color.DarkOrange, 0.0f), 2.0f);

        #endregion

        #region - Initialize -

        public FAButton()
        {
            //base.InitializeComponent();
            //DoubleBuffered = true;
            Cursor = Cursors.Hand;
            FlatAppearance.BorderSize = 0;
            FlatStyle = FlatStyle.Flat;
            Font = new Font("Arial", 11F, FontStyle.Bold);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Calling the base class OnPaint
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            drawBorder(g);
            e.Graphics.FillRoundedRectangle(new LinearGradientBrush(ClientRectangle, ColorTop, ColorBottom, 90F),
                _padding, _padding, Width - (_padding + 3), Height - (_padding + 3), _radius);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                TextRenderer.DrawText(e.Graphics, Text, Font, e.ClipRectangle, ForeColor, flags);
            }
        }

        private void drawBorder(Graphics g) => g.DrawRoundedRectangle(_borderPen, _padding, _padding, Width - (_padding + 3), Height - (_padding + 3), _radius);

        #endregion

    }
}
