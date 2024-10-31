using ChartRealtime.Models;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ChartRealtime
{
    public partial class HistoryForm : Form
    {
        private int currentPage = 1;
        public HistoryForm()
        {
            InitializeComponent();
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Now.AddHours(-1);
            dtpTo.Value = DateTime.Now;
            txtFromWarningPoint.Value = 0;
            txtToWarningPoint.Value = 100;

            // 結果表示テーブル設定
            grdHistory.GridColor = Color.White;
            grdHistory.EnableHeadersVisualStyles = false;
            grdHistory.Columns[0].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[1].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[2].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[3].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[4].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[5].HeaderCell.Style.Font = new Font("MS Gothic", 10.75F, FontStyle.Bold);
            grdHistory.Columns[0].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.Columns[1].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.Columns[2].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.Columns[3].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.Columns[4].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.Columns[5].HeaderCell.Style.BackColor = Color.Black;
            grdHistory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Single;

            Grid_DBindingCboPageSize();
        }
        
        /// <summary>
        /// ページネーションの表示件数コンボボックスの選択肢設定
        /// </summary>
        private void Grid_DBindingCboPageSize()
        {
            DataTable dtSize = new DataTable();
            dtSize.Columns.Add("id", typeof(string));
            dtSize.Columns.Add("name", typeof(string));
            dtSize.Rows.Add("5", "Rows per page: 5");
            dtSize.Rows.Add("10", "Rows per page: 10");
            dtSize.Rows.Add("100", "Rows per page: 100");
            dtSize.Rows.Add("1", "All");

            cboPageSize.DisplayMember = "name";
            cboPageSize.ValueMember = "id";
            cboPageSize.DataSource = dtSize;
            cboPageSize.SelectedIndex = 0;

        }

        #region イベント処理

        /// <summary>
        /// 戻るボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 表示ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilterHistory_Click(object sender, EventArgs e)
        {
            CreatePage(currentPage);
        }
        /// <summary>
        /// 警告データ取得・ページネーション
        /// </summary>
        /// <param name="PageNumber">１ページに当たり表示件数</param>
        public void CreatePage(int PageNumber)
        {
            DataSet dsTable = new DataSet();
            int pageSize = int.Parse(cboPageSize.SelectedValue.ToString());
            WarningHistory history = new WarningHistory();
            dsTable = history.LoadHistory(dtpFrom.Value, dtpTo.Value, float.Parse(txtmM.Text), float.Parse(txtFromWarningPoint.Text), float.Parse(txtToWarningPoint.Text), PageNumber, pageSize);
            if (dsTable == null)
                return;

            int recordCount = int.Parse(dsTable.Tables[1].Rows[0][0].ToString());
            grdHistory.DataSource = dsTable.Tables[0];

            //ページネーション
            double dblPageCount;
            if (int.Parse(cboPageSize.SelectedValue.ToString()) == 1)
            {
                dblPageCount = 1;
            }
            else
            {
                dblPageCount = (double)((decimal)recordCount / Convert.ToDecimal(pageSize));
            }
            int pageCount = (int)Math.Ceiling(dblPageCount);
            if (currentPage - 1 == pageCount && currentPage - 1 > 0)
            {
                currentPage = pageCount;
                lblPage.Text = pageCount + " of " + pageCount;
            }
            else
            {
                lblPage.Text = currentPage + " of " + pageCount;
            }

        }

        /// <summary>
        /// 次のページボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage = currentPage + 1;
            CreatePage(currentPage);
        }

        /// <summary>
        /// 前のページボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage = currentPage - 1;
            }
            CreatePage(currentPage);
        }

        /// <summary>
        /// ページ内件数変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            //件数を変更した後に、最初のページ表示
            currentPage = 1;
        }
        #endregion
    }
}
