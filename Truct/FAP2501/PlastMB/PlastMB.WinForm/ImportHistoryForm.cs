
using PlastMB.Common;
using PlastMB.Dialog;
using PlastMB.Helper;
using PlastMB.Model;
using PlastMB.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using PlastMB.CustomModels.Dtos;
using System.Net.Http;
using System.Threading.Tasks;
using PlastMB.CustomModels.SearchConditions;


namespace PlastMB
{
    public partial class ImportHistoryForm : BaseForm
    {
        #region - Definition -

        private FileHelper? watcher;

        private readonly BindingList<TrnOperationOee> dataHistoryCsv = new BindingList<TrnOperationOee>();
        private readonly BindingList<TrnImportHistory> dataHistoryImport = new BindingList<TrnImportHistory>();
        private readonly BindingList<TrnOperationOee> dataOperationOee = new BindingList<TrnOperationOee>();
        private readonly BindingList<TrnOperationResult> dataOperationResult = new BindingList<TrnOperationResult>();

        private List<Data> listForces = new List<Data>();

        // List of rows with force n = 1
        private readonly List<int> indexs = new List<int>();
        // List of rows will paint green background
        private readonly List<int> rows = new List<int>();
        private bool showLoading = false;

        private readonly WebRequestHelper _webRequestHelper;
        private const string API_SEARCH = "/api/TrnImportHistory/search-History";
        private const string API_SEARCHTrnOperationOee = "/api/TrnOperationOee/search-TrnOperationOee";
        private const string API_SEARCHTrnOperationResult = "/api/TrnOperationResult/search-TrnOperationResult";

        private const string DATEFORMAT = "yyyy/MM/dd";
        private const string DATEFORMAT_FULL = "yyyy/MM/dd HH:mm:ss";
        private DataGridViewCellStyle dataGridViewCellStyle2;
        private int lastRowIndex = -1;

        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region - Initialize -

        public ImportHistoryForm()
        {
            _webRequestHelper = new WebRequestHelper();
            InitializeComponent();

            showLoading = Settings.Default.ShowLoading;
        }

        /// <summary>
        /// Executed when the UI form loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportHistoryForm_Load(object sender, EventArgs e)
        {
            dataGridViewCellStyle2 = new DataGridViewCellStyle()
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
            };

            int centerX = grvHistoryFile.Left + (grvHistoryFile.Width - loadingBox1.Width) / 2;
            int centerY = grvHistoryFile.Top + (grvHistoryFile.Height - loadingBox1.Height) / 2;
            loadingBox1.Location = new Point(centerX, centerY);

            grvHistoryDetail.DataSource = dataOperationOee;

            grvHistoryDetail.Columns[0].DefaultCellStyle.Format = DATEFORMAT;
            grvHistoryDetail.Columns[1].DefaultCellStyle.Format = DATEFORMAT_FULL;
            //grvHistoryDetail.Columns[0].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryDetail.Columns[2].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryDetail.Columns[3].DefaultCellStyle = dataGridViewCellStyle2;
            grvHistoryDetail.Columns[5].DefaultCellStyle = dataGridViewCellStyle2;
            grvHistoryDetail.Columns[6].DefaultCellStyle = dataGridViewCellStyle2;
            grvHistoryDetail.Columns[9].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grvHistoryDetail.ScrollBars = ScrollBars.Both;
            grvHistoryDetail.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryDetail.Columns["AchievementRegistrationDate"].HeaderText = "日付";
            grvHistoryDetail.Columns["AchievementRegistrationTime"].HeaderText = "時刻";
            grvHistoryDetail.Columns["ProcessingTime"].HeaderText = "加工時間";
            grvHistoryDetail.Columns["ProcessingStopTime"].HeaderText = "作業停止時間";
            grvHistoryDetail.Columns["LossStopTime"].HeaderText = "ロス停止時間";
            grvHistoryDetail.Columns["ProductionCount"].HeaderText = "生産件数";
            grvHistoryDetail.Columns["OperationRate"].HeaderText = "稼働率";
            grvHistoryDetail.Columns["EquipmentOperationHours"].HeaderText = "設備稼働時間";
            grvHistoryDetail.Columns["LoadTime"].HeaderText = "負荷時間";
            grvHistoryDetail.Columns["TimeOperatingRate"].HeaderText = "時間稼働率";

            grvHistoryFile.DataSource = dataHistoryImport;

            grvHistoryFile.Columns["ImportTime"].DefaultCellStyle.Format = DATEFORMAT_FULL;
            //grvHistoryFile.Columns[0].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryFile.Columns[2].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryFile.Columns[3].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryFile.Columns[5].DefaultCellStyle = dataGridViewCellStyle2;
            //grvHistoryFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grvHistoryFile.ScrollBars = ScrollBars.Both;
            grvHistoryFile.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;
            grvHistoryFile.Columns[0].Visible = false;
            grvHistoryFile.Columns[2].Visible = false;
            grvHistoryFile.Columns[3].Visible = false;
            grvHistoryFile.Columns[4].Visible = false;
            grvHistoryFile.Columns[8].Visible = false;

            grvHistoryFile.Columns["FileName"].HeaderText = "ファイ名";
            grvHistoryFile.Columns["Status"].HeaderText = "ステータス";
            grvHistoryFile.Columns["Note"].HeaderText = "備考";
            grvHistoryFile.Columns["ImportTime"].HeaderText = "インポート時刻";

            LoadFileHistory();
            LoadDataHistoryDetail();


        }

        #endregion

        #region - Method -


        public async void LoadDataHistoryDetail()
        {
            dataHistoryCsv.Clear();
        }

        public async void LoadFileHistory()
        {
            dataHistoryImport.Clear();
            string fileName = txtFileName.Text;
            string machine = txtMachine.Text;
            DateTime dtFfrom = dtpdFrom.Value.Date;
            DateTime dtTo = dtpdTo.Value.Date;
            await SearchAsyncFileHistory(fileName, dtFfrom, dtTo, machine, 0, 0, 0);
        }


        /// <summary>
        /// Update list CSV in DataGridView
        /// </summary>
        /// <param name="newfile"></param>
        /// <summary>
        /// Show No Excel config dialog
        /// </summary>
        private void ShowDialogNoExcelConfig()
        {
            var message = "Đường dẫn file Excel";
            var title = "Không tìm thấy cấu hình";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            logger.Warn($"{message}");
        }



        /// <summary>
        /// Show Loading dialog
        /// </summary>
        /// <param name="title"></param>
        private void ShowDialogLoading(string title)
        {
            if (InvokeRequired)
            {
                // after we've done all the processing, 
                Invoke(new MethodInvoker(delegate
                {
                    // load the control with the appropriate data
                    ShowDialogLoading(title);
                }));
                return;
            }
            transparentPanel1.Visible = true;
            loadingBox1.Title = title;
            loadingBox1.BringToFront();
            loadingBox1.Visible = true;
        }

        #endregion

        #region -- Load Data
        private async Task SearchAsyncFileHistory(string fileNmame, DateTime dtFrom, DateTime dtTo, string machine, int factory, int pageIndex, int pageSize)
        {
            //LogHelper.Logger.LogInfo("============Start seach information guide ============");

            dataHistoryCsv.Clear();
            //pagination1.Reset();
            try
            {
                // Tạo tham số tìm kiếm
                var searchParam = new TrnImportHistorySearchImpl
                {
                    FileName = fileNmame,
                    MachineNo = machine,
                    FactoryCd = "",
                    Status = "",
                    FromDate = dtFrom,
                    ToDate = dtTo
                };

                // Serialize tham số thành JSON
                var jsonParam = JsonConvert.SerializeObject(searchParam);

                // Gửi yêu cầu POST đến API
                var responseString = await _webRequestHelper.PostAsync(API_SEARCH, jsonParam);

                // Xử lý dữ liệu phản hồi
                var response = JsonConvert.DeserializeObject<GenericResponseResult<TrnImportHistory>>(responseString);

                if (response == null)
                {
                    MessageBox.Show("Không có dữ liệu.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LogHelper.Logger.LogInfo("Không có dữ liệu.");
                }

                // Hiển thị dữ liệu
                ShowDataImportFile(response.ListData, 1, 1);

                //LogHelper.Logger.LogInfo("============End seach information guide============");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu HTTP: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi khi gửi yêu cầu HTTP: " + ex.ToString());
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show($"Lỗi xử lý JSON: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi xử lý JSON: " + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                //LogHelper.Logger.LogError("Error: " + ex.ToString());
            }
        }

        private async Task SearchAsyncTrnOperationResult(string fileNmame, DateTime dtImport, int pageIndex, int pageSize)
        {
            //LogHelper.Logger.LogInfo("============Start seach information guide ============");

            dataOperationResult.Clear();
            //pagination1.Reset();
            try
            {
                // Tạo tham số tìm kiếm
                var searchParam = new TrnImportHistoryDetailSearchImpl
                {
                    FILENAME = fileNmame,
                    IMPORTTIME = dtImport
                };

                // Serialize tham số thành JSON
                var jsonParam = JsonConvert.SerializeObject(searchParam);

                // Gửi yêu cầu POST đến API
                var responseString = await _webRequestHelper.PostAsync(API_SEARCHTrnOperationResult, jsonParam);

                // Xử lý dữ liệu phản hồi
                var response = JsonConvert.DeserializeObject<GenericResponseResult<TrnOperationResult>>(responseString);

                if (response == null)
                {
                    MessageBox.Show("Không có dữ liệu.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LogHelper.Logger.LogInfo("Không có dữ liệu.");
                }

                // Hiển thị dữ liệu
                ShowDataTrnOperationResult(response.ListData, 1, 1);

                //LogHelper.Logger.LogInfo("============End seach information guide============");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu HTTP: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi khi gửi yêu cầu HTTP: " + ex.ToString());
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show($"Lỗi xử lý JSON: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi xử lý JSON: " + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                //LogHelper.Logger.LogError("Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalPages"></param>
        private void ShowDataTrnOperationResult(List<TrnOperationResult> data, int pageIndex, int totalPages)
        {
            try
            {
                if (data != null)
                {
                    int index = 1;
                    foreach (var api in data)
                    {
                        var item = new TrnOperationResult
                        {
                            AchievementRegistrationDate = api.AchievementRegistrationDate,
                            AchievementRegistrationTime = api.AchievementRegistrationTime,
                            MachineNo = api.MachineNo,
                            ProcessingStartTime = api.ProcessingStartTime,
                            ProcessingEndTime = api.ProcessingEndTime,
                            ProcessingTime = ConvertIntToHouse(api.ProcessingTime),
                            ProgressRate = api.ProgressRate,
                            StandardTime = ConvertIntToHouse(api.StandardTime),
                            ProcessingStopTime = ConvertIntToHouse(api.ProcessingStopTime),
                            LossStopTime = ConvertIntToHouse(api.LossStopTime),
                            SlipNo = api.SlipNo,
                            CustomerName = api.CustomerName,
                            DueDate = api.DueDate,
                            ProductName1 = api.ProductName1,
                            ProductName2 = api.ProductName2,
                            Value = api.Value,
                            Operator1 = api.Operator1,
                            Operator2 = api.Operator2,
                            Operator3 = api.Operator3,
                            MeasurementInspection = api.MeasurementInspection,
                            Changeover = api.Changeover,
                            CAD = api.CAD,
                            EquipmentFailure = api.EquipmentFailure,
                            Cleaning = api.Cleaning,
                            RestTime = api.RestTime

                        };

                        dataOperationResult.Add(item);
                        index++;
                    }
                }
                grvHistoryDetail.DataBindings.Clear();
                grvHistoryDetail.DataSource = dataOperationResult;
                grvHistoryDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                grvHistoryDetail.Columns[3].DefaultCellStyle.Format = DATEFORMAT_FULL;
                grvHistoryDetail.Columns[4].DefaultCellStyle.Format = DATEFORMAT_FULL;
                grvHistoryDetail.Columns[12].DefaultCellStyle.Format = DATEFORMAT;

                grvHistoryDetail.Columns[2].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[6].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[15].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[19].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[20].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[21].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[22].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[23].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns["AchievementRegistrationDate"].HeaderText = "日付";
                grvHistoryDetail.Columns["AchievementRegistrationTime"].HeaderText = "時刻";
                grvHistoryDetail.Columns["MachineNo"].HeaderText = "設備No";
                grvHistoryDetail.Columns["ProcessingStartTime"].HeaderText = "作業開始";
                grvHistoryDetail.Columns["ProcessingTime"].HeaderText = "所要時間" ;
                grvHistoryDetail.Columns["ProcessingEndTime"].HeaderText = "作業完了";
                grvHistoryDetail.Columns["ProgressRate"].HeaderText = "進捗率";
                grvHistoryDetail.Columns["StandardTime"].HeaderText = "標準時間";
                grvHistoryDetail.Columns["ProcessingStopTime"].HeaderText = "作業停止時間";
                grvHistoryDetail.Columns["LossStopTime"].HeaderText = "ロス停止時間";
                grvHistoryDetail.Columns["SlipNo"].HeaderText = "伝票No";
                grvHistoryDetail.Columns["CustomerName"].HeaderText = "得意先名";
                grvHistoryDetail.Columns["DueDate"].HeaderText = "納期";
                grvHistoryDetail.Columns["ProductName1"].HeaderText = "品名1";
                grvHistoryDetail.Columns["ProductName2"].HeaderText = "品名2";
                grvHistoryDetail.Columns["Value"].HeaderText = "個数";
                grvHistoryDetail.Columns["Operator1"].HeaderText = "オペレータ1";
                grvHistoryDetail.Columns["Operator2"].HeaderText = "オペレータ2";
                grvHistoryDetail.Columns["Operator3"].HeaderText = "オペレータ3";
                grvHistoryDetail.Columns["MeasurementInspection"].HeaderText = "測定検査";
                grvHistoryDetail.Columns["Changeover"].HeaderText = "段取替え";
                grvHistoryDetail.Columns["CAD"].HeaderText = "CAD";
                grvHistoryDetail.Columns["EquipmentFailure"].HeaderText = "設備故障";
                grvHistoryDetail.Columns["Cleaning"].HeaderText = "清掃";
                grvHistoryDetail.Columns["RestTime"].HeaderText = "休憩時間";

                // Cập nhật thông tin phân trang
                //pagination1.SetInfo(pageIndex, totalPages);
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có exception
                var message = $"Đã xảy ra lỗi khi hiển thị dữ liệu: {ex.Message}";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // LogHelper.Logger.LogError("Error: " + ex.ToString());
            }

            // Xử lý hiển thị trạng thái loading
            var thread = new Thread(() =>
            {
                Thread.Sleep(100 * 4);
                loadingBox1.Invoke((MethodInvoker)(() => loadingBox1.Visible = false));
            });
            thread.Start();
        }



        private async Task SearchAsyncTrnOperationOee(string fileNmame, DateTime dtImport, int pageIndex, int pageSize)
        {
            //LogHelper.Logger.LogInfo("============Start seach information guide ============");

            dataOperationOee.Clear();
            //pagination1.Reset();
            try
            {
                // Tạo tham số tìm kiếm
                var searchParam = new TrnImportHistoryDetailSearchImpl
                {
                    FILENAME = fileNmame,
                    IMPORTTIME = dtImport
                };

                // Serialize tham số thành JSON
                var jsonParam = JsonConvert.SerializeObject(searchParam);

                // Gửi yêu cầu POST đến API
                var responseString = await _webRequestHelper.PostAsync(API_SEARCHTrnOperationOee, jsonParam);

                // Xử lý dữ liệu phản hồi
                var response = JsonConvert.DeserializeObject<GenericResponseResult<TrnOperationOee>>(responseString);

                if (response == null)
                {
                    MessageBox.Show("Không có dữ liệu.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LogHelper.Logger.LogInfo("Không có dữ liệu.");
                }

                // Hiển thị dữ liệu
                ShowDataTrnOperationOee(response.ListData, 1, 1);

                //LogHelper.Logger.LogInfo("============End seach information guide============");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu HTTP: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi khi gửi yêu cầu HTTP: " + ex.ToString());
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show($"Lỗi xử lý JSON: {ex.Message}");
                //LogHelper.Logger.LogError("Lỗi xử lý JSON: " + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                //LogHelper.Logger.LogError("Error: " + ex.ToString());
            }
        }

        private void ShowDataTrnOperationOee(List<TrnOperationOee> data, int pageIndex, int totalPages)
        {
            try
            {
                if (data != null)
                {
                    int index = 1;
                    foreach (var api in data)
                    {
                        var item = new TrnOperationOee
                        {
                            AchievementRegistrationDate = api.AchievementRegistrationDate,
                            AchievementRegistrationTime = api.AchievementRegistrationTime,
                            ProcessingTime = ConvertIntToHouse(api.ProcessingTime),
                            ProcessingStopTime = ConvertIntToHouse(api.ProcessingStopTime),
                            LossStopTime = ConvertIntToHouse(api.LossStopTime),
                            ProductionCount = api.ProductionCount,
                            OperationRate = api.OperationRate,
                            EquipmentOperationHours = ConvertIntToHouse(api.EquipmentOperationHours),
                            LoadTime = ConvertIntToHouse(api.LoadTime),
                            TimeOperatingRate = api.TimeOperatingRate

                        };

                        dataOperationOee.Add(item);
                        index++;
                    }
                }
                grvHistoryDetail.DataBindings.Clear();
                grvHistoryDetail.DataSource = dataOperationOee;
                grvHistoryDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                grvHistoryDetail.Columns[5].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[6].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns[9].DefaultCellStyle = dataGridViewCellStyle2;
                grvHistoryDetail.Columns["AchievementRegistrationDate"].HeaderText = "日付";
                grvHistoryDetail.Columns["AchievementRegistrationTime"].HeaderText = "時刻";
                grvHistoryDetail.Columns["ProcessingTime"].HeaderText = "加工時間";
                grvHistoryDetail.Columns["ProcessingStopTime"].HeaderText = "作業停止時間";
                grvHistoryDetail.Columns["LossStopTime"].HeaderText = "ロス停止時間";
                grvHistoryDetail.Columns["ProductionCount"].HeaderText = "生産件数";
                grvHistoryDetail.Columns["OperationRate"].HeaderText = "稼働率";
                grvHistoryDetail.Columns["EquipmentOperationHours"].HeaderText = "設備稼働時間";
                grvHistoryDetail.Columns["LoadTime"].HeaderText = "負荷時間";
                grvHistoryDetail.Columns["TimeOperatingRate"].HeaderText = "時間稼働率";

                // Cập nhật thông tin phân trang
                //pagination1.SetInfo(pageIndex, totalPages);
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có exception
                var message = $"Đã xảy ra lỗi khi hiển thị dữ liệu: {ex.Message}";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // LogHelper.Logger.LogError("Error: " + ex.ToString());
            }

            // Xử lý hiển thị trạng thái loading
            var thread = new Thread(() =>
            {
                Thread.Sleep(100 * 4);
                loadingBox1.Invoke((MethodInvoker)(() => loadingBox1.Visible = false));
            });
            thread.Start();
        }

        private string ConvertIntToHouse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "00:00:00";
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(int.Parse(value));
                string result = time.ToString(@"hh\:mm\:ss");
                return result;
            }
        }

        private void ShowDataImportFile(List<TrnImportHistory> data, int pageIndex, int totalPages)
        {
            try
            {
                if (data != null)
                {
                    int index = 1;
                    foreach (var api in data)
                    {
                        var item = new TrnImportHistory
                        {
                            ID = api.ID,
                            FactoryCd = api.FactoryCd,
                            FileName = api.FileName,
                            MachineNo = api.MachineNo,
                            Status = api.Status,
                            Note = api.Note,
                            ImportTime = api.ImportTime,
                            Flag = api.Flag

                        };

                        dataHistoryImport.Add(item);
                        grvHistoryFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        index++;
                    }
                }
                // Cập nhật thông tin phân trang
                //pagination1.SetInfo(pageIndex, totalPages);
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có exception
                var message = $"Đã xảy ra lỗi khi hiển thị dữ liệu: {ex.Message}";
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // LogHelper.Logger.LogError("Error: " + ex.ToString());
            }

            // Xử lý hiển thị trạng thái loading
            var thread = new Thread(() =>
            {
                Thread.Sleep(100 * 4);
                loadingBox1.Invoke((MethodInvoker)(() => loadingBox1.Visible = false));
            });
            thread.Start();
        }
        #endregion

        #region - Event -

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex >= 4 && indexs.IndexOf(e.RowIndex) != -1)
            {
                grvHistoryDetail.Cursor = Cursors.Hand;
            }
            else
            {
                grvHistoryDetail.Cursor = Cursors.Default;
            }
        }

        private async void grvHistoryFile_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int index = e.RowIndex;
            if (lastRowIndex == index) return;

            string flag = dataHistoryImport[index].Flag;
            if (flag.Equals("1"))
            {
                await SearchAsyncTrnOperationOee(dataHistoryImport[index].FileName, dataHistoryImport[index].ImportTime, 1, 1);
            }
            else
            {
                await SearchAsyncTrnOperationResult(dataHistoryImport[index].FileName, dataHistoryImport[index].ImportTime, 1, 1);
            }
            lastRowIndex = index;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var i = indexs.IndexOf(e.RowIndex);

        }

        private void titleBar1_MouseDowned(object sender, EventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void titleBar1_CloseClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void faButton1_Click(object sender, EventArgs e)
        {
            dataOperationOee.Clear();
            dataOperationResult.Clear();
            LoadFileHistory();
        }

        private void faButton2_Click(object sender, EventArgs e)
        {
            txtFileName.Text = string.Empty;
            txtMachine.Text = string.Empty;
            dtpdFrom.Value = DateTime.Now;
            dtpdTo.Value = DateTime.Now;

        }

        private void faButton3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HAForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //StopWatcher();
                watcher?.Stop();
                Utility.ShowForm(Constant.MenuTitle);
            }
            catch (Exception ex)
            {
                logger.Error(Utility.GetExceptionInfo(ex, "EAForm.cs"));
            }
        }

        #endregion
    }
}