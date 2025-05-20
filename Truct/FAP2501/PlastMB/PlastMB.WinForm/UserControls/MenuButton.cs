//==================================================================================================
// System  : DenKa
// File    : MenuButton.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Menu Button
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using PlastMB.Extension;
using PlastMB.Helper;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
public partial class MenuButton : UserControl
{
    #region - Definition -

    private string _title = string.Empty;
    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
            Invalidate();
        }
    }

    private readonly int _padding = 10;
    private int _radius = 20;
    public int Radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
            Invalidate();
        }
    }

    public Color ColorTop
    {
        get; set;
    } = Color.FromArgb(255, 235, 218);
    public Color ColorBottom
    {
        get; set;
    } = Color.FromArgb(255, 190, 135);

    private Color _borderColor = Color.DarkOrange;
    private Pen _borderPen = new Pen(ControlPaint.Light(SystemColors.Control, 0.0f), 0);
    public Color BorderColor
    {
        get
        {
            return _borderColor;
        }
        set
        {
            _borderColor = value;
            _borderPen = new Pen(ControlPaint.Light(_borderColor, 0.0f), _borderWidth);
            Invalidate();
        }
    }

    private float _borderWidth = 3.0f;
    public float BorderWidth
    {
        get
        {
            return _borderWidth;
        }
        set
        {
            _borderWidth = value;
            _borderPen = new Pen(ControlPaint.Light(_borderColor, 0.0f), _borderWidth);
            Invalidate();
        }
    }

    private System.Timers.Timer resettimer;

    #endregion

    #region - Initialize -

    public MenuButton()
    {
        InitializeComponent();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        drawBorder(g);
        //drawBackground(g);

        e.Graphics.FillRoundedRectangle(new LinearGradientBrush(ClientRectangle, ColorTop, ColorBottom, 90F),
            _padding, _padding, Width - (_padding * 2), Height - (_padding * 2), _radius);
    }

    private void drawBorder(Graphics g) => g.DrawRoundedRectangle(_borderPen, _padding, _padding, Width - (_padding * 2), Height - (_padding * 2), _radius);

    private void MenuButton_Load(object sender, System.EventArgs e)
    {
        this.label1.Text = _title;
    }

    #endregion

    #region - Method -

    public void ShowNumber(int value)
    {
        var text = value.ToString();
        var padding = 17;
        if (value > 9 && value <= 99)
        {
            padding = 13;
        }
        else if (value > 99)
        {
            padding = 10;
            text = "99+";
        }
        this.infoBadge1.SetPropertyThreadSafe(() => infoBadge1.Text, text);
        if (value > 0)
        {
            this.infoBadge1.Padding = new Padding(padding, 11, 0, 0);
            this.infoBadge1.SetPropertyThreadSafe(() => infoBadge1.Visible, true);
        }
        else
        {
            this.infoBadge1.SetPropertyThreadSafe(() => infoBadge1.Visible, false);
        }
    }

    private void HoverBorder()
    {
        resettimer?.Stop();
        if (ColorTop != Color.FromArgb(255, 190, 135))
        {
            ColorTop = Color.FromArgb(255, 190, 135);
            Invalidate();
        }
    }

    private void ResetBorder()
    {
        if (ColorTop != Color.FromArgb(255, 235, 218))
        {
            ColorTop = Color.FromArgb(255, 235, 218);
            Invalidate();
        }
    }

    #endregion

    #region - Event -

    private void MenuButton_MouseHover(object sender, System.EventArgs e)
    {
        HoverBorder();
    }

    private void MenuButton_MouseLeave(object sender, System.EventArgs e)
    {
        resettimer = new System.Timers.Timer();
        resettimer.Elapsed += (sen, events) => ResetBorder();
        resettimer.Interval = 100;
        resettimer.AutoReset = false;
        resettimer.Start();
    }

    private void label1_MouseHover(object sender, System.EventArgs e)
    {
        HoverBorder();
    }

    private void label1_MouseLeave(object sender, System.EventArgs e)
    {
        resettimer = new System.Timers.Timer();
        resettimer.Elapsed += (sen, events) => ResetBorder();
        resettimer.Interval = 100;
        resettimer.AutoReset = false;
        resettimer.Start();
    }

    private void label1_Click(object sender, System.EventArgs e)
    {
        base.OnClick(e);
    }

    #endregion
}
}