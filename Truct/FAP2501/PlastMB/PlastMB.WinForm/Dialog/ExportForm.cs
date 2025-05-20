using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PlastMB.Common;
using PlastMB.Model;
using NLog;

namespace PlastMB.Dialog
{
    public partial class ExportForm : Form
    {
        #region - Definition -

        private readonly bool isBeforeCheck;

        private readonly List<CSVInfo> datas;

        private readonly BindingList<State> source = new BindingList<State>();

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public ExportForm(List<CSVInfo> dt, bool isBeforeCheck = false)
        {
            InitializeComponent();

            this.datas = dt;
            this.isBeforeCheck = isBeforeCheck;
        }

        #endregion

        #region - Method -

        private void LoadData()
        {
            logger.Info($"Export completed: {datas.Count}");
            for (var i = 0; i < datas.Count; i++)
            {
                var dt = new State
                {
                    Index = i + 1,
                    CSV = datas[i].File != null ? datas[i].File.Name : "",
                    InputState = GetState(datas[i].InputState),
                    Excel = datas[i].Excel,
                    ExportState = GetState(datas[i].ExportState, true),
                };
                source.Add(dt);
                logger.Debug(dt);
            }
            dataGridView1.DataSource = source;

            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[0].MinimumWidth = 50;
            //dataGridView1.Columns[1].Width = 330;
            dataGridView1.Columns[1].MinimumWidth = 330;
            dataGridView1.Columns[3].Width = 150;
            dataGridView1.Columns[3].MinimumWidth = 150;
        }

        private string GetState(ExportState state, bool isExportState = false)
        {
            var issue = "-";
            switch (state)
            {
                case ExportState.OK:
                    // issue = "-";
                    if (isExportState && !isBeforeCheck)
                        issue = "Export dữ liệu thành công!";
                    break;

                // Input Issue
                case ExportState.WrongLength:
                    if (!isExportState)
                        issue = "【Length】không đúng định dạng";
                    break;
                case ExportState.WrongCase4:
                    if (!isExportState)
                        issue = "Không có định dạng tên 【LOT】-【NL】";
                    break;
                case ExportState.WrongCase3:
                    if (!isExportState)
                        issue = "Không có định dạng tên 【LOT】-【Color】-【Length】";
                    break;
                case ExportState.WrongCase2:
                    if (!isExportState)
                        issue = "Không có định dạng tên 【LOT】-【Color】";
                    break;
                case ExportState.WrongCase1:
                    // Tệp csv này không có định dạng tên thuộc Case 1 【LOT】
                    // Tệp csv này không có định dạng tên 【LOT】
                    if (!isExportState)
                        issue = "Không có định dạng tên 【LOT】";
                    break;
                case ExportState.WrongCase:
                    if (!isExportState)
                        issue = "Không đúng định dạng tên tệp";
                    break;

                case ExportState.MeasureInvalid:
                    // Tệp csv có giá trị đo có giá trị không phù hợp (0 hoặc null)
                    if (!isExportState)
                        issue = "Giá trị đo có giá trị không phù hợp (0 hoặc null)";
                    break;
                case ExportState.WrongColor:
                    // Tệp csv có LotNo không đúng định dạng: Không đúng mã màu
                    if (!isExportState)
                        issue = "Không đúng mã màu";
                    break;
                case ExportState.WrongLine:
                    // Tệp csv có LotNo không đúng định dạng: Line không bằng 1 hoặc 2
                    if (!isExportState)
                        issue = "【LOT】 không đúng định dạng!";
                    break;
                case ExportState.MissLine:
                    // Tệp csv có LotNo không đúng định dạng: thiếu Line
                    if (!isExportState)
                        issue = "【LOT】 không đúng định dạng.";
                    break;
                case ExportState.WrongLotNo:
                    // Tệp csv có LotNo không đúng định dạng: [0-9][A-L]\d{2}
                    if (!isExportState)
                        issue = "【LOT】 không đúng định dạng";
                    break;
                case ExportState.FileMissType:
                    // Tệp csv có tên không đúng định dạng: không có lực đo
                    if (!isExportState)
                        issue = "Không đúng định dạng";
                    break;
                case ExportState.ExcelMismatch:
                    // Tệp csv có tên không đúng định dạng
                    if (isExportState)
                        issue = "Không tìm thấy tệp Excel tương ứng";
                    break;
                //case ExportState.ExcelMissType:
                case ExportState.FileMalformed:
                    // Tệp csv có tên không đúng định dạng
                    if (!isExportState)
                        issue = "Không đúng định dạng";
                    break;
                case ExportState.FileNotFound:
                    issue = "Không tìm thấy tệp Excel.";
                    break;
                case ExportState.FileOpening:
                    if (isExportState)
                        issue = "Tệp Excel đang được mở bởi một ứng dụng khác";
                    else
                        issue = "Tệp CSV đang được mở bởi một ứng dụng khác";
                    break;

                // Export Issue
                case ExportState.Error:
                    issue = "Export dữ liệu lỗi";      // không thành công
                    break;
                case ExportState.PassIncorrect:
                    issue = "Password không đúng";
                    break;
                case ExportState.Exist:
                    issue = "Các thông số LotNo đã có trong tệp Excel";
                    break;
                case ExportState.Full:
                    issue = "Đã hết cột để thêm dữ liệu";
                    break;
                case ExportState.NotFound:
                    issue = "Hệ thống không tìm thấy giá trị tương ứng để export";
                    break;
                case ExportState.ExportCraft:
                    issue = "Dữ liệu này không được tạo bằng hệ thống";
                    break;
                case ExportState.ExportCraftSuccess:
                    issue = "Thêm dữ liệu vào kết quả đo cũ thành công!";
                    break;

                default:
                    break;
            }
            return issue;
        }

        #endregion

        #region - Event -

        private void ExportForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void faButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

    }
}