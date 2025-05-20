//==================================================================================================
// System  : DenKa
// File    : TitleBar.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Title bar customization
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
public partial class TitleBar : UserControl
{
    #region - Definition -

    // Events
    public event EventHandler CloseClicked;   //Default event
    public event EventHandler MouseDowned;

    // Variables
    private int x;
    private int y;
    private int wid;
    private int hei;
    private string m_title = "Hệ thống truyền dữ liệu tự động";

    [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string Title
    {
        get => m_title;
        set => m_title = value;
    }

    #endregion

    #region - Initialize -

    public TitleBar()
    {
        InitializeComponent();
    }

    private void TitleBar_Load(object sender, EventArgs e)
    {
        label1.Text = m_title;
        toolTip1.SetToolTip(button1, "Thu nhỏ");
        toolTip1.SetToolTip(button3, "Đóng");
        if (ParentForm?.WindowState == FormWindowState.Normal)
        {
            button2.Image = Properties.Resources.maximize;
            toolTip1.SetToolTip(button2, "Phóng to");
        }
        else if (ParentForm?.WindowState == FormWindowState.Maximized)
        {
            button2.Image = Properties.Resources.restore;
            toolTip1.SetToolTip(button2, "Khôi phục");
            wid = ParentForm.MinimumSize.Width;
            hei = ParentForm.MinimumSize.Height;
            if (wid == 0) wid = 1024;
            if (hei == 0) hei = 600;
            x = (Screen.PrimaryScreen.WorkingArea.Width - wid) / 2;
            y = (Screen.PrimaryScreen.WorkingArea.Height - hei) / 2;
            ParentForm.Location = new System.Drawing.Point(0, 0);
            ParentForm.ClientSize = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            ParentForm.WindowState = FormWindowState.Normal;
        }
    }

    #endregion

    #region - Event -

    private void TitleBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && MouseDowned != null)
        {
            MouseDowned.Invoke(sender, e);
        }
    }

    private void label1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && MouseDowned != null)
        {
            MouseDowned.Invoke(sender, e);
        }
    }

    private void button1_Click(object sender, System.EventArgs e)
    {
        ParentForm.WindowState = FormWindowState.Minimized;
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
        if (ParentForm.ClientSize.Width < Screen.PrimaryScreen.WorkingArea.Width)
        {
            x = ParentForm.Location.X;
            y = ParentForm.Location.Y;
            wid = ParentForm.ClientSize.Width;
            hei = ParentForm.ClientSize.Height;
            ParentForm.Location = new System.Drawing.Point(0, 0);
            ParentForm.ClientSize = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            button2.Image = Properties.Resources.restore;
            toolTip1.SetToolTip(button2, "Khôi phục");
        }
        else
        {
            ParentForm.WindowState = FormWindowState.Normal;
            ParentForm.Location = new System.Drawing.Point(x, y);
            ParentForm.ClientSize = new System.Drawing.Size(wid, hei);
            button2.Image = Properties.Resources.maximize;
            toolTip1.SetToolTip(button2, "Phóng to");
        }
    }

    private void button3_Click(object sender, System.EventArgs e)
    {
        CloseClicked?.Invoke(sender, e);
    }

    #endregion
}
}