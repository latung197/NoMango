using ChartRealtime.Models;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartRealtime
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// リアタイム表示フラグ
        /// </summary>
        private bool isTaskRun = false;

        /// <summary>
        /// リアルタイム表示チェックボックス
        /// </summary>
        private bool isRealTime = true;

        /// <summary>
        /// リアルタイム表示周期
        /// </summary>
        private int timeLoad = int.Parse(ConfigurationManager.AppSettings.Get("timeLoad"));

        /// <summary>
        /// 初期表示で現時点前の表示データ量
        /// </summary>
        private int timeView = int.Parse(ConfigurationManager.AppSettings.Get("timeView"));

        /// <summary>
        /// Bo危険度・警告のX軸の最大値
        /// </summary>
        private int maxAxBO = 15000;

        /// <summary>
        /// Bo危険度・警告のX軸の最小値
        /// </summary>
        private int minAxBO = 0;

        /// <summary>
        /// Bo危険度・警告のX軸間隔
        /// </summary>
        private int intervalBO = 500;

        /// <summary>
        /// 東短辺BOグラフ取得時点
        /// </summary>
        private long dTimeBOE;

        /// <summary>
        /// 西短辺BOグラフ取得時点
        /// </summary>
        private long dTimeBOW;

        /// <summary>
        /// 初期凝固遅れ・再溶解データ取得時点
        /// </summary>
        private long dTimeTrandMelt;

        /// <summary>
        /// 東短辺の熱電対温度データの取得時点
        /// </summary>
        private long dTimeEastElectrodeTempN;
        private long dTimeEastElectrodeTempO;

        /// <summary>
        /// 西短辺の熱電対温度データの取得時点
        /// </summary>
        private long dTimeWesternElectrodeTempN;
        private long dTimeWesternElectrodeTempO;

        /// <summary>
        /// 東短辺の温度変化速度の取得時点
        /// </summary>
        private long dTimeEastRateTempChangeN;
        /// <summary>
        /// 東短辺の温度変化速度の取得時点
        /// </summary>
        private long dTimeEastRateTempChangeO;

        /// <summary>
        /// 西短辺の温度変化速度の取得時点
        /// </summary>
        private long dTimeWesternRateTempChangeN;
        /// <summary>
        /// 西短辺の温度変化速度の取得時点
        /// </summary>
        private long dTimeWesternRateTempChangeO;

        /// <summary>
        /// BO危険度・警告のグラフデータ
        /// </summary>
        private DataSet dsMdlresBO = null;

        /// <summary>
        /// 東短辺熱電対温度のグラフデータ
        /// </summary>
        private DataSet dsEastElectrodeTemp = null;
        /// <summary>
        /// 西短辺熱電対温度のグラフデータ
        /// </summary>
        private DataSet dsWesternElectrodeTemp = null;
        /// <summary>
        /// 東短辺の温度変化速度のグラフデータ
        /// </summary>
        private DataSet dsEastRateTempChange = null;

        /// <summary>
        /// 西短辺の温度変化速度のグラフデータ
        /// </summary>
        private DataSet dsWesternRateTempChange = null;

        private static Thread taskStartForm = null;
        public MainForm()
        {
            try
            {

                //Splash起動
                taskStartForm = new Thread(new ThreadStart(StartForm));
                taskStartForm.IsBackground = true;
                taskStartForm.Start();
                Thread.Sleep(1000);
                InitializeComponent();
                //各グラフ初期
                CreateChrEastBO();
                CreateChrWesternBO();
                CreateChrEastElectrodeTemp();
                CreateChrEastRateTempChange();
                CreateChrWesternElectrodeTemp();
                CreateChrWesternRateTempChange();

                dTimeBOE = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeBOW = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeTrandMelt = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeEastElectrodeTempN = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeEastElectrodeTempO = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeWesternElectrodeTempN = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeWesternElectrodeTempO = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeEastRateTempChangeN = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeEastRateTempChangeO = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeWesternRateTempChangeN = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));
                dTimeWesternRateTempChangeO = long.Parse(DateTime.Now.AddMinutes(-timeView).ToString("yyyyMMddHHmmss"));



                //グラフデータ取得
                Mdlres mdlres = new Mdlres();
                Mdlvar mdlvar = new Mdlvar();
                dsMdlresBO = mdlres.LoadRealTimeBO(dTimeBOE, dTimeBOW, dTimeTrandMelt);
                dsEastElectrodeTemp = mdlvar.LoadRealTimeTemp(dTimeEastElectrodeTempN, dTimeEastElectrodeTempO, "E");
                dsWesternElectrodeTemp = mdlvar.LoadRealTimeTemp(dTimeWesternElectrodeTempN, dTimeWesternElectrodeTempO, "W");
                dsEastRateTempChange = mdlvar.LoadRealTimeTempChange(dTimeEastRateTempChangeN, dTimeEastRateTempChangeO, "E");
                dsWesternRateTempChange = mdlvar.LoadRealTimeTempChange(dTimeWesternRateTempChangeN, dTimeWesternRateTempChangeO, "W");
                taskStartForm.Abort();
               
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();
                taskStartForm.Abort();
                InitializeComponent();
                CreateChrEastBO();
                CreateChrWesternBO();
                CreateChrEastElectrodeTemp();
                CreateChrEastRateTempChange();
                CreateChrWesternElectrodeTemp();
                CreateChrWesternRateTempChange();
            }
        }


    //    bool _initialized = false;
    //    protected override void SetVisibleCore(bool value) =>
    //base.SetVisibleCore(value && _initialized);

        private void MainForm_Load(object sender, EventArgs e)
        {
            //グラフ表示
            StartViewChart();
        }
        /// <summary>
        /// 初期表示の時にグラフ表示アクション
        /// </summary>
        private async void StartViewChart()
        {
            Action myActionView = () =>
            {
                LoadViewChart();
            };
            Task task = new Task(myActionView);
            task.Start();
            await task;
            MainRealTime();
        }
        /// <summary>
        /// Splash起動
        /// </summary>
        private void StartForm()
        {
            try
            {
                Application.Run(new SpashForm());
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
        }

        /// <summary>
        /// 初期表示の時にグラフ表示
        /// </summary>
        private void LoadViewChart()
        {
            // BO危険度・警告表示（東・西）
            // Xử lý hiển thị 
            //E01 || W01
            if (dsMdlresBO != null && dsMdlresBO.Tables.Count > 0)
            {
                if (dsMdlresBO.Tables[0].Rows.Count > 0)
                {
                    dTimeBOE = long.Parse(dsMdlresBO.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                    CreatePointChrEastBO(dsMdlresBO);
                }
                if (dsMdlresBO.Tables[1].Rows.Count > 0)
                {
                    dTimeBOW = long.Parse(dsMdlresBO.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                    CreatePointChrWesternBO(dsMdlresBO);
                }

                if (dsMdlresBO.Tables[2].Rows.Count > 0)
                {
                    dTimeTrandMelt = long.Parse(dsMdlresBO.Tables[2].Compute("max([DTIME])", string.Empty).ToString());

                }
            }


            // E02 | W02
            if (dsEastElectrodeTemp != null && dsEastElectrodeTemp.Tables.Count > 0)
            {
                CreatePointChrEastElectrodeTemp(dsEastElectrodeTemp);
                if (dsEastElectrodeTemp.Tables[0].Rows.Count > 0)
                {
                    dTimeEastElectrodeTempN = long.Parse(dsEastElectrodeTemp.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                }
                else
                {
                    if (dsEastElectrodeTemp.Tables[1].Rows.Count > 0)
                    {
                        dTimeEastElectrodeTempO = long.Parse(dsEastElectrodeTemp.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                    }
                }
            }

            if (dsWesternElectrodeTemp != null && dsWesternElectrodeTemp.Tables.Count > 0)
            {
                CreatePointChrWesternElectrodeTemp(dsWesternElectrodeTemp);
                if (dsWesternElectrodeTemp.Tables[0].Rows.Count > 0)
                {
                    dTimeWesternElectrodeTempN = long.Parse(dsWesternElectrodeTemp.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                }
                else
                {
                    if (dsWesternElectrodeTemp.Tables[1].Rows.Count > 0)
                    {
                        dTimeWesternElectrodeTempO = long.Parse(dsWesternElectrodeTemp.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                    }
                }
            }

            //E03| W03
            if (dsEastRateTempChange != null && dsEastRateTempChange.Tables.Count > 0)
            {
                CreatePointChrEastRateTempChange(dsEastRateTempChange);
                if (dsEastRateTempChange.Tables[0].Rows.Count > 0)
                {
                    dTimeEastRateTempChangeN = long.Parse(dsEastRateTempChange.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                }
                else
                {
                    if (dsEastRateTempChange.Tables[1].Rows.Count > 0)
                    {
                        dTimeEastRateTempChangeO = long.Parse(dsEastRateTempChange.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                    }
                }
            }

            if (dsWesternRateTempChange != null && dsWesternRateTempChange.Tables.Count > 0)
            {
                CreatePointChrWesternRateTempChange(dsWesternRateTempChange);
                if (dsWesternRateTempChange.Tables[0].Rows.Count > 0)
                {
                    dTimeWesternRateTempChangeN = long.Parse(dsWesternRateTempChange.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                }
                else
                {
                    if (dsWesternRateTempChange.Tables[1].Rows.Count > 0)
                    {
                        dTimeWesternRateTempChangeO = long.Parse(dsWesternRateTempChange.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                    }
                }
            }
            // Dispose
            if (dsMdlresBO != null && dsEastElectrodeTemp != null && dsEastRateTempChange != null && dsWesternRateTempChange != null)
            {
                dsMdlresBO.Dispose();
                dsEastElectrodeTemp.Dispose();
                dsWesternElectrodeTemp.Dispose();
                dsEastRateTempChange.Dispose();
                dsWesternRateTempChange.Dispose();
            }
        }

        /// <summary>
        /// グラフリアルタイム表示
        /// </summary>
        private async void MainRealTime()
        {
            // リアルタイム表示タスク実行されているかチェック
            if (!isTaskRun)
            {
                isTaskRun = true;

                Mdlres mdlres = new Mdlres();
                Mdlvar mdlvar = new Mdlvar();
                await Task.Run(() =>
                {
                    while (isRealTime)
                    {
                        //グラフデータ取得
                        dsMdlresBO = mdlres.LoadRealTimeBO(dTimeBOE, dTimeBOW, dTimeTrandMelt);
                        dsEastElectrodeTemp = mdlvar.LoadRealTimeTemp(dTimeEastElectrodeTempN, dTimeEastElectrodeTempO, "E");
                        dsWesternElectrodeTemp = mdlvar.LoadRealTimeTemp(dTimeWesternElectrodeTempN, dTimeWesternElectrodeTempO, "W");
                        dsEastRateTempChange = mdlvar.LoadRealTimeTempChange(dTimeEastRateTempChangeN, dTimeEastRateTempChangeO, "E");
                        dsWesternRateTempChange = mdlvar.LoadRealTimeTempChange(dTimeWesternRateTempChangeN, dTimeWesternRateTempChangeO, "W");

                        //グラフ表示
                        if (dsMdlresBO != null && dsMdlresBO.Tables.Count > 0)
                        {
                            if (dsMdlresBO.Tables[0].Rows.Count > 0)
                            {
                                dTimeBOE = long.Parse(dsMdlresBO.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                                CreatePointChrEastBO(dsMdlresBO);
                            }

                            if (dsMdlresBO.Tables[1].Rows.Count > 0)
                            {
                                dTimeBOW = long.Parse(dsMdlresBO.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                                CreatePointChrWesternBO(dsMdlresBO);
                            }

                            if (dsMdlresBO.Tables[2].Rows.Count > 0)
                            {
                                dTimeTrandMelt = long.Parse(dsMdlresBO.Tables[2].Compute("max([DTIME])", string.Empty).ToString());
                            }
                        }

                        if (dsEastElectrodeTemp != null && dsEastElectrodeTemp.Tables.Count > 0)
                        {
                            CreatePointChrEastElectrodeTemp(dsEastElectrodeTemp);

                            if (dsEastElectrodeTemp.Tables[0].Rows.Count > 0)
                            {
                                dTimeEastElectrodeTempN = long.Parse(dsEastElectrodeTemp.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                            }
                            else
                            {
                                if (dsEastElectrodeTemp.Tables[1].Rows.Count > 0)
                                {
                                    dTimeEastElectrodeTempO = long.Parse(dsEastElectrodeTemp.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                                }
                            }
                        }

                        if (dsWesternElectrodeTemp != null && dsWesternElectrodeTemp.Tables.Count > 0)
                        {
                            CreatePointChrWesternElectrodeTemp(dsWesternElectrodeTemp);
                            if (dsWesternElectrodeTemp.Tables[0].Rows.Count > 0)
                            {
                                dTimeWesternElectrodeTempN = long.Parse(dsWesternElectrodeTemp.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                            }
                            else
                            {
                                if (dsWesternElectrodeTemp.Tables[1].Rows.Count > 0)
                                {
                                    dTimeWesternElectrodeTempO = long.Parse(dsWesternElectrodeTemp.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                                }
                            }
                        }

                        if (dsEastRateTempChange != null && dsEastRateTempChange.Tables.Count > 0)
                        {
                            CreatePointChrEastRateTempChange(dsEastRateTempChange);

                            if (dsEastRateTempChange.Tables[0].Rows.Count > 0)
                            {
                                dTimeEastRateTempChangeN = long.Parse(dsEastRateTempChange.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                            }
                            else
                            {
                                if (dsEastRateTempChange.Tables[1].Rows.Count > 0)
                                {
                                    dTimeEastRateTempChangeO = long.Parse(dsEastRateTempChange.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                                }
                            }
                        }

                        if (dsWesternRateTempChange != null && dsWesternRateTempChange.Tables.Count > 0)
                        {
                            CreatePointChrWesternRateTempChange(dsWesternRateTempChange);

                            if (dsWesternRateTempChange.Tables[0].Rows.Count > 0)
                            {
                                dTimeWesternRateTempChangeN = long.Parse(dsWesternRateTempChange.Tables[0].Compute("max([DTIME])", string.Empty).ToString());
                            }
                            else
                            {
                                if (dsWesternRateTempChange.Tables[1].Rows.Count > 0)
                                {
                                    dTimeWesternRateTempChangeO = long.Parse(dsWesternRateTempChange.Tables[1].Compute("max([DTIME])", string.Empty).ToString());
                                }
                            }
                        }
                        // Dispose
                        if (dsMdlresBO != null && dsEastElectrodeTemp != null && dsEastRateTempChange != null && dsWesternRateTempChange != null)
                        {
                            dsMdlresBO.Dispose();
                            dsEastElectrodeTemp.Dispose();
                            dsWesternElectrodeTemp.Dispose();
                            dsEastRateTempChange.Dispose();
                            dsWesternRateTempChange.Dispose();
                        }
                        Thread.Sleep(timeLoad);
                    }
                });
                isTaskRun = false;
            }
        }

        #region 各グラフ描画
        /// <summary>
        /// 東短辺BO危険度・警告
        /// </summary>
        /// <param name="dsTable">グラフデータ</param>
        private void CreatePointChrEastBO(DataSet dsTable)
        {

            chrEastBO.Invoke(new Action(() =>

            {
                Series series0 = chrEastBO.Series["警告"];
                Series series2 = chrEastBO.Series["危険度"];

                if (dsTable.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow _row in dsTable.Tables[0].Rows)
                    {

                        float pointXZValue = 0;
                        int pointCount = 0;

                        float XZ = float.Parse(_row["XZ"].ToString());

                        pointCount = series0.Points.Count;
                        if (pointCount != 0)
                        {
                            pointXZValue = float.Parse(series0.Points[pointCount - 1].XValue.ToString());
                        }
                        float pointYZValue = float.Parse(_row["XZ"].ToString());
                        if (float.Parse(_row["XZ"].ToString()) < pointXZValue)
                        {
                            series0.Points.Clear();
                            series2.Points.Clear();

                            //chrEastBO.ChartAreas.Clear();
                            //CreateChrEastBO();

                            chrEastBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
                            chrEastBO.ChartAreas[0].AxisX.Minimum = 0;

                            series0.Points.AddXY(_row["XZ"], double.Parse(_row["ZBOE"].ToString()));
                            series2.Points.AddXY(_row["XZ"], double.Parse(_row["NFBO_E"].ToString()));
                            chrEastBO.ChartAreas[0].RecalculateAxesScale();

                            chrEastBO.ChartAreas[0].AxisX.ScaleView.Zoom(0, maxAxBO);

                            chrEastBO.Update();

                        }
                        else
                        {

                            series0.Points.AddXY(_row["XZ"], double.Parse(_row["ZBOE"].ToString()));
                            series2.Points.AddXY(_row["XZ"], double.Parse(_row["NFBO_E"].ToString()));
                            float t1 = float.Parse(_row["XZ"].ToString());
                            if (t1 > maxAxBO)
                            {


                                chrEastBO.ChartAreas[0].AxisX.Maximum = t1+10;
                                chrEastBO.ChartAreas[0].AxisX.Minimum = 0;
                                ///chrEastBO.ChartAreas[0].AxisX.Minimum = double.NaN;
                                chrEastBO.ChartAreas[0].RecalculateAxesScale();
                                chrEastBO.ChartAreas[0].AxisX.ScaleView.Zoom(t1 - maxAxBO, t1);

                                chrEastBO.Update();


                            }
                        }
                    }
                }

                //if (dsTable.Tables[2].Rows.Count > 0)
                //{
                //    foreach (DataRow _row in dsTable.Tables[2].Rows)
                //    {

                //        float pointXZValue = 0;
                //        int pointCount = 0;

                //        pointCount = chrEastBO.Series["再溶解危険度"].Points.Count;
                //        if (pointCount != 0)
                //        {
                //            pointXZValue = float.Parse(chrEastBO.Series["再溶解危険度"].Points[pointCount - 1].XValue.ToString());
                //        }
                //        if (float.Parse(_row["XZ"].ToString()) < pointXZValue && _row["ZOHE"] != null && _row["ZOHE"].ToString() != "")
                //        {
                //            chrEastBO.Series["再溶解危険度"].Points.Clear();
                //            chrEastBO.Series["初期凝固遅れ危険度"].Points.Clear();
                //            chrEastBO.ChartAreas[0].RecalculateAxesScale();
                //            chrEastBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
                //            chrEastBO.ChartAreas[0].AxisX.Minimum = 0;
                //            chrEastBO.ChartAreas[0].AxisX.ScaleView.Zoom(0, maxAxBO);
                //            chrEastBO.Series["再溶解危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZOHE"].ToString()));
                //            chrEastBO.Series["初期凝固遅れ危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZRME"].ToString()));
                //            chrEastBO.Update();

                //        }
                //        else
                //        {
                //            if (_row["ZOHE"] != null && _row["ZRME"] != null && _row["ZOHE"].ToString() != "" && _row["ZRME"].ToString() != "")
                //            {
                //                chrEastBO.Series["再溶解危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZOHE"].ToString()));
                //                chrEastBO.Series["初期凝固遅れ危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZRME"].ToString()));
                //                if (float.Parse(_row["XZ"].ToString()) > maxAxBO)
                //                {
                //                    chrEastBO.ChartAreas[0].AxisX.Maximum = float.Parse(_row["XZ"].ToString());
                //                    //chrEastBO.ChartAreas[0].AxisX.ScaleView.Zoom(float.Parse(_row["XZ"].ToString()) - maxAxBO, float.Parse(_row["XZ"].ToString()));
                //                }
                //            }
                //        }
                //    }
                //}
            }));
        }

        /// <summary>
        /// 西短辺BO危険度・警告
        /// </summary>
        /// <param name="dsTable"> グラフデータ</param>
        private void CreatePointChrWesternBO(DataSet dsTable)
        {


            chrWesternBO.Invoke(
              new Action(() =>
              {

                  if (dsTable.Tables[0].Rows.Count > 0)
                  {
                      foreach (DataRow _row in dsTable.Tables[1].Rows)
                      {
                          float pointXZValue = 0;
                          int pointCount = 0;
                          pointCount = chrWesternBO.Series["警告"].Points.Count;
                          if (pointCount != 0)
                          {
                              pointXZValue = float.Parse(chrWesternBO.Series["警告"].Points[pointCount - 1].XValue.ToString());
                          }
                          if (float.Parse(_row["XZ"].ToString()) < pointXZValue)
                          {
                              chrWesternBO.ChartAreas[0].RecalculateAxesScale();
                              chrWesternBO.Series["警告"].Points.Clear();
                              chrWesternBO.Series["危険度"].Points.Clear();
                              chrWesternBO.Series["警告"].Points.AddXY(_row["XZ"], double.Parse(_row["ZBOW"].ToString()));
                              chrWesternBO.Series["危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["NFBO_W"].ToString()));
                              chrWesternBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
                              chrWesternBO.ChartAreas[0].AxisX.Minimum = 0;
                          }
                          else
                          {
                              chrWesternBO.Series["警告"].Points.AddXY(_row["XZ"], double.Parse(_row["ZBOW"].ToString()));
                              chrWesternBO.Series["危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["NFBO_W"].ToString()));
                              if (float.Parse(_row["XZ"].ToString()) >= maxAxBO)
                              {
                                  double n = Math.Ceiling(double.Parse(_row["XZ"].ToString()));

                                  chrWesternBO.ChartAreas[0].AxisX.ScaleView.Zoom(float.Parse(_row["XZ"].ToString()) - maxAxBO, float.Parse(_row["XZ"].ToString()));
                              }
                          }
                      }
                  }


                  if (dsTable.Tables[2].Rows.Count > 0)
                  {
                      foreach (DataRow _row in dsTable.Tables[2].Rows)
                      {

                          float pointXZValue = 0;
                          int pointCount = 0;
                          pointCount = chrWesternBO.Series["再溶解危険度"].Points.Count;
                          if (pointCount != 0)
                          {
                              pointXZValue = float.Parse(chrWesternBO.Series["再溶解危険度"].Points[pointCount - 1].XValue.ToString());
                          }
                          if (float.Parse(_row["XZ"].ToString()) < pointXZValue)
                          {
                              chrWesternBO.ChartAreas[0].RecalculateAxesScale();
                              chrWesternBO.Series["再溶解危険度"].Points.Clear();
                              chrWesternBO.Series["初期凝固遅れ危険度"].Points.Clear();
                              chrWesternBO.Series["再溶解危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZOHW"].ToString()));
                              chrWesternBO.Series["初期凝固遅れ危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZRMW"].ToString()));
                              chrWesternBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
                              chrWesternBO.ChartAreas[0].AxisX.Minimum = 0;
                          }
                          else
                          {
                              chrWesternBO.Series["再溶解危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZOHW"].ToString()));
                              chrWesternBO.Series["初期凝固遅れ危険度"].Points.AddXY(_row["XZ"], double.Parse(_row["ZRMW"].ToString()));
                              if (float.Parse(_row["XZ"].ToString()) >= maxAxBO)
                              {
                                  chrWesternBO.ChartAreas[0].AxisX.ScaleView.Zoom(float.Parse(_row["XZ"].ToString()) - maxAxBO, float.Parse(_row["XZ"].ToString()));
                                  chrWesternBO.ChartAreas[0].AxisX.Maximum = float.Parse(_row["XZ"].ToString());
                                  chrWesternBO.ChartAreas[0].RecalculateAxesScale();
                              }
                          }
                      }
                  }

              }));
        }

        /// <summary>
        /// 東短辺熱電対温度描画
        /// </summary>
        /// <param name="ds">グラフデータ</param>
        private void CreatePointChrEastElectrodeTemp(DataSet ds)
        {
            chrEastElectrodeTemp.Invoke(new Action(() =>
            {
                if (ds == null || ds.Tables.Count == 0)
                {
                    return;
                }
                if (ds.Tables[0].Rows.Count != 0)
                {
                    chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Points.Clear();

                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if ((ds.Tables[0].Columns[i].ColumnName).Contains("TTCE_Z") == true)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                            {
                                chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.NaN);
                                continue;
                            }
                            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                        }
                    }
                }

                if (ds.Tables[1].Rows.Count != 0)
                {
                    chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Points.Clear();
                    for (int i = 1; i < ds.Tables[1].Columns.Count; i++)
                    {
                        if ((ds.Tables[1].Columns[i].ColumnName).Contains("TTCE_Z") == true)
                        {
                            if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                            {
                                chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.NaN);
                                continue;
                            }
                            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 西短辺熱電対温度描画
        /// </summary>
        /// <param name="ds">グラフデータ</param>
        private void CreatePointChrWesternElectrodeTemp(DataSet ds)
        {
            chrWesternElectrodeTemp.Invoke(new Action(() =>
            {
                if (ds == null || ds.Tables.Count == 0)
                {
                    return;
                }
                if (ds.Tables[0].Rows.Count != 0)
                {
                    chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Points.Clear();

                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if ((ds.Tables[0].Columns[i].ColumnName).Contains("TTCW_Z") == true)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                            {
                                chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.NaN);
                                continue;
                            }
                            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                        }
                    }
                }

                if (ds.Tables[1].Rows.Count != 0)
                {
                    chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Points.Clear();
                    for (int i = 0; i < ds.Tables[1].Columns.Count; i++)
                    {
                        if ((ds.Tables[1].Columns[i].ColumnName).Contains("TTCW_Z") == true)
                        {
                            if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                            {
                                chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.NaN);
                                continue;
                            }
                            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 東短辺温度変化速度描画
        /// </summary>
        /// <param name="ds">グラフデータ</param>
        private void CreatePointChrEastRateTempChange(DataSet ds)
        {
            chrEastRateTempChange.Invoke(new Action(() =>
            {
                if (ds == null || ds.Tables.Count == 0)
                {
                    return;
                }
                if (ds.Tables[0].Rows.Count != 0)
                {
                    chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Points.Clear();
                    chrEastRateTempChange.ChartAreas[0].AxisY.Maximum = 6;
                    chrEastRateTempChange.ChartAreas[0].AxisY.Minimum = -2;
                    double yMax = 6;
                    double yMin = -2;

                    for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if ((ds.Tables[0].Columns[i].ColumnName).Contains("VTCE_Z") == true)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                            {
                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.NaN);
                            }
                            else
                            {
                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                                if (double.Parse(ds.Tables[0].Rows[0][i].ToString()) >= yMax)
                                {
                                    chrEastRateTempChange.ChartAreas[0].AxisY.Maximum = double.Parse(ds.Tables[0].Rows[0][i].ToString()) + 1;
                                    yMax = double.Parse(ds.Tables[0].Rows[0][i].ToString());
                                }
                                if (double.Parse(ds.Tables[0].Rows[0][i].ToString()) <= yMin)
                                {
                                    chrEastRateTempChange.ChartAreas[0].AxisY.Minimum = double.Parse(ds.Tables[0].Rows[0][i].ToString()) - 1;
                                    yMin = double.Parse(ds.Tables[0].Rows[0][i].ToString());
                                }
                            }
                        }
                    }
                }

                if (ds.Tables[1].Rows.Count != 0)
                {
                    chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Points.Clear();
                    chrEastRateTempChange.ChartAreas[0].AxisY.Maximum = 6;
                    chrEastRateTempChange.ChartAreas[0].AxisY.Minimum = -2;
                    double yMax = 6;
                    double yMin = -2;
                    for (int i = 1; i < ds.Tables[1].Columns.Count; i++)
                    {
                        if ((ds.Tables[1].Columns[i].ColumnName).Contains("VTCE_Z") == true)
                        {
                            if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                            {
                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.NaN);
                            }
                            else
                            {

                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                                if (double.Parse(ds.Tables[1].Rows[0][i].ToString()) >= yMax)
                                {
                                    chrEastRateTempChange.ChartAreas[0].AxisY.Maximum = double.Parse(ds.Tables[1].Rows[0][i].ToString()) + 1;
                                    yMax = double.Parse(ds.Tables[1].Rows[0][i].ToString());
                                }
                                if (double.Parse(ds.Tables[1].Rows[0][i].ToString()) <= yMin)
                                {
                                    chrEastRateTempChange.ChartAreas[0].AxisY.Minimum = double.Parse(ds.Tables[1].Rows[0][i].ToString()) - 1;
                                    yMin = double.Parse(ds.Tables[1].Rows[0][i].ToString());
                                }
                            }
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 西短辺温度変化速度描画
        /// </summary>
        /// <param name="ds">グラフデータ</param>
        private void CreatePointChrWesternRateTempChange(DataSet ds)
        {
            chrWesternRateTempChange.Invoke(new Action(() =>
            {
                if (ds == null || ds.Tables.Count == 0)
                {
                    return;
                }
                if (ds.Tables[0].Rows.Count != 0)
                {

                    chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Points.Clear();
                    chrWesternRateTempChange.ChartAreas[0].AxisY.Maximum = 6;
                    chrWesternRateTempChange.ChartAreas[0].AxisY.Minimum = -2;
                    double yMax = 6;
                    double yMin = -2;

                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if ((ds.Tables[0].Columns[i].ColumnName).Contains("VTCW_Z") == true)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                            {
                                chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.NaN);
                            }
                            else
                            {
                                chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));

                                if (double.Parse(ds.Tables[0].Rows[0][i].ToString()) >= yMax)
                                {
                                    chrWesternRateTempChange.ChartAreas[0].AxisY.Maximum = double.Parse(ds.Tables[0].Rows[0][i].ToString()) + 1;
                                    yMax = double.Parse(ds.Tables[0].Rows[0][i].ToString());
                                }
                                if (double.Parse(ds.Tables[0].Rows[0][i].ToString()) <= yMin)
                                {
                                    chrWesternRateTempChange.ChartAreas[0].AxisY.Minimum = double.Parse(ds.Tables[0].Rows[0][i].ToString()) - 1;
                                    yMin = double.Parse(ds.Tables[0].Rows[0][i].ToString());
                                }

                            }
                        }
                    }
                }

                if (ds.Tables[1].Rows.Count != 0)
                {
                    double yMax = 6;
                    double yMin = -2;
                    chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Points.Clear();
                    chrWesternRateTempChange.ChartAreas[0].AxisY.Maximum = yMax;
                    chrWesternRateTempChange.ChartAreas[0].AxisY.Minimum = yMin;

                    for (int i = 0; i < ds.Tables[1].Columns.Count; i++)
                    {
                        if ((ds.Tables[0].Columns[i].ColumnName).Contains("VTCW_Z") == true)
                        {
                            if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                            {
                                chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.NaN);
                            }
                            else
                            {
                                chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                                if (double.Parse(ds.Tables[1].Rows[0][i].ToString()) >= yMax)
                                {
                                    chrWesternRateTempChange.ChartAreas[0].AxisY.Maximum = double.Parse(ds.Tables[1].Rows[0][i].ToString()) + 1;
                                    yMax = double.Parse(ds.Tables[1].Rows[0][i].ToString());
                                }
                                if (double.Parse(ds.Tables[1].Rows[0][i].ToString()) <= yMin)
                                {
                                    chrWesternRateTempChange.ChartAreas[0].AxisY.Minimum = double.Parse(ds.Tables[1].Rows[0][i].ToString()) - 1;
                                    yMin = double.Parse(ds.Tables[1].Rows[0][i].ToString());
                                }
                            }

                        }
                    }
                }
            }));
        }

        #endregion 各グラフ描画

        #region 表示条件で各グラフ描画
        /// <summary>
        /// 東短辺BO危険度・警告を表示条件で表示
        /// </summary>
        /// <param name="dsTable">グラフデータ</param>
        private void LoadPointChrEastBO(DataSet dsTable)
        {
            chrEastBO.Series["警告"].Points.Clear();
            chrEastBO.Series["危険度"].Points.Clear();
            chrEastBO.Series["再溶解危険度"].Points.Clear();
            chrEastBO.Series["初期凝固遅れ危険度"].Points.Clear();

            if (dsTable == null || dsTable.Tables.Count == 0)
                return;

            int i = 1;
            foreach (DataRow _row in dsTable.Tables[0].Rows)
            {
                chrEastBO.Series["警告"].Points.AddXY(i, double.Parse(_row[1].ToString()));
                i++;
            }

            foreach (DataRow _row in dsTable.Tables[1].Rows)
            {
                chrEastBO.Series["危険度"].Points.AddXY(i, double.Parse(_row[1].ToString()));
                i++;
            }
            chrEastBO.ChartAreas[0].AxisX.Maximum = dsTable.Tables[0].Rows.Count;
            chrEastBO.ChartAreas[0].AxisX.LabelStyle.Interval = 20;
        }

        /// <summary>
        /// 西短辺BO危険度・警告を表示条件で表示
        /// </summary>
        /// <param name="dsTable">グラフデータ</param>
        private void LoadPointChrWesternBO(DataSet dsTable)
        {
            chrWesternBO.Series["警告"].Points.Clear();
            chrWesternBO.Series["危険度"].Points.Clear();

            if (dsTable == null || dsTable.Tables[1].Rows.Count == 0)
                return;
            int i = 1;
            foreach (DataRow _row in dsTable.Tables[1].Rows)
            {
                chrWesternBO.Series["警告"].Points.AddXY(i, double.Parse(_row[0].ToString()));
                chrWesternBO.Series["危険度"].Points.AddXY(i, double.Parse(_row[1].ToString()));
                i++;
            }
            chrWesternBO.ChartAreas[0].AxisX.Minimum = 0;
            chrWesternBO.ChartAreas[0].AxisX.Maximum = 100;
            chrWesternBO.ChartAreas[0].AxisX.LabelStyle.Interval = 20;
        }

        /// <summary>
        /// 東短辺熱電対温度描画
        /// </summary>
        /// <param name="dFrom">期間（FROM)</param>
        /// <param name="dTo">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        private void LoadPointChrEastElectrodeTemp(long dFrom, long dTo, float mM)
        {
            Mdlvar mdlvar = new Mdlvar();
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Points.Clear();

            using (DataSet ds = mdlvar.LoadTemp(dFrom, dTo, mM, "E"))
            {
                chrEastElectrodeTemp.Invoke(new Action(() =>
                {
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        return;
                    }

                    if (ds.Tables[0].Rows.Count != 0)
                    {

                        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        {
                            if ((ds.Tables[0].Columns[i].ColumnName).Contains("TTCE_Z") == true)
                            {
                                if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                            }
                        }
                    }

                    if (ds.Tables[1].Rows.Count != 0)
                    {
                        chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Points.Clear();

                        for (int i = 0; i < ds.Tables[1].Columns.Count; i++)
                        {
                            if ((ds.Tables[1].Columns[i].ColumnName).Contains("TTCE_Z") == true)
                            {

                                if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                            }
                        }
                    }
                }));
            }
        }
        
        /// <summary>
        /// 西短辺熱電対温度描画
        /// </summary>
        /// <param name="dFrom">期間（FROM)</param>
        /// <param name="dTo">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        private void LoadPointChrWesternElectrodeTemp(long dFrom, long dTo, float mM)
        {
            Mdlvar mdlvar = new Mdlvar();

            using (DataSet ds = mdlvar.LoadTemp(dFrom, dTo, mM, "W"))
            {
                chrWesternElectrodeTemp.Invoke(new Action(() =>
                {
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        return;
                    }

                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Points.Clear();

                        for (int i = 1; i <= ds.Tables[0].Columns.Count - 2; i++)
                        {
                            if ((ds.Tables[0].Columns[i].ColumnName).Contains("TTCW_Z") == true)
                            {
                                if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));

                            }
                        }
                    }

                    if (ds.Tables[1].Rows.Count != 0)
                    {
                        chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Points.Clear();

                        for (int i = 1; i <= ds.Tables[1].Columns.Count - 2; i++)
                        {
                            if ((ds.Tables[0].Columns[i].ColumnName).Contains("TTCW_Z") == true)
                            {
                                if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                            }
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// 東短辺温度変化速度描画
        /// </summary>
        /// <param name="dFrom">期間（FROM)</param>
        /// <param name="dTo">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        private void LoadPointChrEastRateTempChange(long dFrom, long dTo, float mM)
        {
            Mdlvar mdlvar = new Mdlvar();
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Points.Clear();

            using (DataSet ds = mdlvar.LoadRateTempChange(dFrom, dTo, mM, "E"))
            {
                chrEastRateTempChange.Invoke(new Action(() =>
                {
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        return;
                    }

                    if (ds.Tables[0].Rows.Count != 0)
                    {

                        for (int i = 1; i <= ds.Tables[0].Columns.Count - 2; i++)
                        {
                            if ((ds.Tables[0].Columns[i].ColumnName).Contains("VTCE_Z") == true)
                            {
                                if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Points.AddXY(double.Parse(ds.Tables[0].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                            }
                        }
                    }

                    if (ds.Tables[1].Rows.Count != 0)
                    {
                        chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Points.Clear();

                        for (int i = 1; i <= ds.Tables[1].Columns.Count - 2; i++)
                        {
                            if ((ds.Tables[1].Columns[i].ColumnName).Contains("VTCE_Z") == true)
                            {
                                if (ds.Tables[1].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                                {
                                    continue;
                                }
                                chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Points.AddXY(double.Parse(ds.Tables[1].Columns[i].ColumnName.Substring(6)), double.Parse(ds.Tables[1].Rows[0][i].ToString()));

                            }
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// 西短辺温度変化速度描画
        /// </summary>
        /// <param name="dFrom">期間（FROM)</param>
        /// <param name="dTo">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        private void LoadPointChrWesternRateTempChange(long dFrom, long dTo, float mM)
        {
            Mdlvar mdlvar = new Mdlvar();

            using (DataSet ds = mdlvar.LoadRateTempChange(dFrom, dTo, mM, "W"))
            {
                chrWesternRateTempChange.Invoke(new Action(() =>
                {
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        return;
                    }
                    int xi = 40;
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Points.Clear();

                        for (int i = 1; i <= ds.Tables[0].Columns.Count - 2; i++)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[0].Rows[0][i].ToString()) == 0)
                            {
                                continue;
                            }
                            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Points.AddXY(xi, double.Parse(ds.Tables[0].Rows[0][i].ToString()));
                            xi = xi + 20;
                        }
                    }

                    if (ds.Tables[1].Rows.Count != 0)
                    {
                        chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Points.Clear();
                        xi = 40;
                        for (int i = 1; i <= ds.Tables[1].Columns.Count - 2; i++)
                        {
                            if (ds.Tables[0].Rows[0][i].ToString() == "" || double.Parse(ds.Tables[1].Rows[0][i].ToString()) == 0)
                            {
                                continue;
                            }
                            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Points.AddXY(xi, double.Parse(ds.Tables[1].Rows[0][i].ToString()));
                            xi = xi + 20;
                        }
                    }
                }));
            }
        }

        #endregion

        #region 各グラフの属性設定

        /// <summary>
        /// 東短辺BO危険度・警告
        /// </summary>
        private void CreateChrEastBO()
        {

            chrEastBO.BackColor = Color.Black;
            chrEastBO.Legends.Clear();
            chrEastBO.Series.Clear();

            chrEastBO.Series.Add("警告");
            chrEastBO.Series.Add("危険度");
            chrEastBO.Series.Add("再溶解危険度");
            chrEastBO.Series.Add("初期凝固遅れ危険度");

            chrEastBO.Series["警告"].ChartType = SeriesChartType.Spline;
            chrEastBO.Series["警告"].MarkerSize = 10; 
            //chrEastBO.Series["警告"].MarkerStyle = MarkerStyle.Circle; 
            chrEastBO.Series["警告"].Color = Color.FromArgb(0, 125, 241); // Blue line
            chrEastBO.Series["警告"].BorderWidth = 2;
            chrEastBO.Series["警告"].IsValueShownAsLabel = false;

            chrEastBO.Series["危険度"].ChartType = SeriesChartType.StepLine;
            chrEastBO.Series["危険度"].MarkerSize = 10;
            //chrEastBO.Series["危険度"].MarkerStyle = MarkerStyle.Circle;
            chrEastBO.Series["危険度"].Color = Color.FromArgb(241, 139, 0); // Yellow line
            chrEastBO.Series["危険度"].BorderWidth = 2;

            chrEastBO.Series["再溶解危険度"].ChartType = SeriesChartType.Spline;
            chrEastBO.Series["再溶解危険度"].Color = Color.Red;// Red line
            chrEastBO.Series["再溶解危険度"].MarkerSize = 10;
            //chrEastBO.Series["再溶解危険度"].MarkerStyle = MarkerStyle.Circle;
            chrEastBO.Series["再溶解危険度"].BorderWidth = 1;

            chrEastBO.Series["初期凝固遅れ危険度"].ChartType = SeriesChartType.Spline;
            chrEastBO.Series["初期凝固遅れ危険度"].Color = Color.White; // White line
            chrEastBO.Series["初期凝固遅れ危険度"].MarkerSize = 10;
            //chrEastBO.Series["初期凝固遅れ危険度"].MarkerStyle = MarkerStyle.Circle;
            chrEastBO.Series["初期凝固遅れ危険度"].BorderWidth = 1;

            // Areas設定
            chrEastBO.ChartAreas[0].BackColor = Color.Black;
            chrEastBO.ChartAreas[0].BorderWidth = 2;
            chrEastBO.ChartAreas[0].BorderColor = Color.White;
            chrEastBO.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrEastBO.ChartAreas[0].Position = new ElementPosition(0, 0, 100, 100);

            // X軸
            chrEastBO.ChartAreas[0].AxisX.Minimum = minAxBO;
            chrEastBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
            chrEastBO.ChartAreas[0].AxisX.Interval = intervalBO;
            chrEastBO.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrEastBO.ChartAreas[0].AxisX.Title = "鋳造長";
            chrEastBO.ChartAreas[0].AxisX.LineColor = Color.White;
            chrEastBO.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrEastBO.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrEastBO.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Bold);
            chrEastBO.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrEastBO.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.Black;
            chrEastBO.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrEastBO.ChartAreas[0].AxisX.LabelStyle.Interval = intervalBO;
            chrEastBO.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
             chrEastBO.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chrEastBO.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chrEastBO.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chrEastBO.ChartAreas[0].AxisX.ScrollBar.BackColor = Color.White;

            // Y軸
            chrEastBO.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastBO.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Bold);
            chrEastBO.ChartAreas[0].AxisY.Minimum = 0;
            chrEastBO.ChartAreas[0].AxisY.Maximum = 120;
            chrEastBO.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chrEastBO.ChartAreas[0].AxisY.Title = "危険度" + Environment.NewLine + "警告";
            chrEastBO.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrEastBO.ChartAreas[0].AxisY.LineColor = Color.White;
            chrEastBO.ChartAreas[0].AxisY.LabelStyle.Interval = 20;
            chrEastBO.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastBO.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chrEastBO.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
        }

        /// <summary>
        /// 西短辺BO危険度・警告
        /// </summary>
        private void CreateChrWesternBO()
        {
            // Series
            chrWesternBO.BackColor = Color.Black;
            chrWesternBO.Legends.Clear();
            chrWesternBO.Series.Clear();
            chrWesternBO.Series.Add("警告");
            chrWesternBO.Series.Add("危険度");
            chrWesternBO.Series.Add("再溶解危険度");
            chrWesternBO.Series.Add("初期凝固遅れ危険度");

            chrWesternBO.Series["警告"].ChartType = SeriesChartType.Spline;
            chrWesternBO.Series["警告"].MarkerSize = 10;
            //chrWesternBO.Series["警告"].MarkerStyle = MarkerStyle.Circle;
            chrWesternBO.Series["警告"].Color = Color.FromArgb(0, 125, 241);
            chrWesternBO.Series["警告"].BorderWidth = 2;

            chrWesternBO.Series["危険度"].ChartType = SeriesChartType.StepLine;
            chrEastBO.Series["危険度"].MarkerSize = 10;
            //chrEastBO.Series["危険度"].MarkerStyle = MarkerStyle.Circle;
            chrWesternBO.Series["危険度"].MarkerSize = 10;
            //chrWesternBO.Series["危険度"].MarkerStyle =  MarkerStyle.Circle;
            chrWesternBO.Series["危険度"].Color = Color.FromArgb(241, 139, 0);
            chrWesternBO.Series["危険度"].BorderWidth = 2;

            chrWesternBO.Series["再溶解危険度"].ChartType = SeriesChartType.Spline;
            chrWesternBO.Series["再溶解危険度"].MarkerSize = 10;
            //chrWesternBO.Series["再溶解危険度"].MarkerStyle = MarkerStyle.Circle;
            chrWesternBO.Series["再溶解危険度"].Color = Color.Red;
            chrWesternBO.Series["再溶解危険度"].BorderWidth = 2;

            chrWesternBO.Series["初期凝固遅れ危険度"].ChartType = SeriesChartType.Spline;
            chrWesternBO.Series["初期凝固遅れ危険度"].MarkerSize = 10;
            //chrWesternBO.Series["初期凝固遅れ危険度"].MarkerStyle = MarkerStyle.Circle;
            chrWesternBO.Series["初期凝固遅れ危険度"].Color = Color.White;
            chrWesternBO.Series["初期凝固遅れ危険度"].BorderWidth = 2;

            // Areas
            chrWesternBO.ChartAreas[0].BorderColor = Color.White;
            chrWesternBO.ChartAreas[0].BorderWidth = 2;
            chrWesternBO.ChartAreas[0].BackColor = Color.Black;
            chrWesternBO.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrWesternBO.ChartAreas[0].Position = new ElementPosition(0, 0, 100, 100);

            // X軸
            chrWesternBO.ChartAreas[0].AxisX.Maximum = maxAxBO;
            chrWesternBO.ChartAreas[0].AxisX.Minimum = minAxBO;
            chrWesternBO.ChartAreas[0].AxisX.Interval = intervalBO;
            chrWesternBO.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrWesternBO.ChartAreas[0].AxisX.Title = "鋳造長";
            chrWesternBO.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisX.LineColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrWesternBO.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Bold);
            chrWesternBO.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrWesternBO.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.Gray;
            chrWesternBO.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chrWesternBO.ChartAreas[0].AxisX.LabelStyle.Interval = intervalBO;
            chrWesternBO.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
            chrWesternBO.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chrWesternBO.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chrWesternBO.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chrWesternBO.ChartAreas[0].AxisX.ScrollBar.BackColor = Color.White;

            // Y軸
            chrWesternBO.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 10F, System.Drawing.FontStyle.Bold);
            chrWesternBO.ChartAreas[0].AxisY.Minimum = 0;
            chrWesternBO.ChartAreas[0].AxisY.Maximum = 120;
            chrWesternBO.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chrWesternBO.ChartAreas[0].AxisY.Title = "危険度" + Environment.NewLine + "警告";
            chrWesternBO.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisY.LineColor = Color.White;
            chrWesternBO.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chrWesternBO.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chrWesternBO.ChartAreas[0].AxisY.LabelStyle.Interval = 20;
            chrWesternBO.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

        }

        /// <summary>
        /// 東短辺熱電対温度
        /// </summary>
        private void CreateChrEastElectrodeTemp()
        {
            chrEastElectrodeTemp.BackColor = Color.Black;
            chrEastElectrodeTemp.Legends.Clear();
            chrEastElectrodeTemp.Series.Clear();

            chrEastElectrodeTemp.Series.Add("短辺熱電対温度推定値TTCE");
            chrEastElectrodeTemp.Series.Add("短辺熱電対温度推定値TTCE2");

            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].ChartType = SeriesChartType.Spline;
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].Color = Color.FromArgb(0, 125, 241);
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE"].BorderWidth = 2;
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].ChartType = SeriesChartType.Spline;
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].Color = Color.FromArgb(241, 139, 0);
            chrEastElectrodeTemp.Series["短辺熱電対温度推定値TTCE2"].BorderWidth = 2;

            chrEastElectrodeTemp.ChartAreas[0].BorderWidth = 2;
            chrEastElectrodeTemp.ChartAreas[0].BorderColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].BackColor = Color.Black;
            chrEastElectrodeTemp.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrEastElectrodeTemp.ChartAreas[0].Position = new ElementPosition(6, 0, 93, 100);

            // X軸
            chrEastElectrodeTemp.ChartAreas[0].AxisX.Minimum = 40;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.Maximum = 340;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.LineColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MajorTickMark.Interval = 1;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.Interval = 20;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.Title = "熱電対位置";
            chrEastElectrodeTemp.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            chrEastElectrodeTemp.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrEastElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.White;

            // Y軸
            chrEastElectrodeTemp.ChartAreas[0].AxisY.Maximum = 300;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.Minimum = 50;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.Interval = 50;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.Title = "熱電対温度（℃）";
            chrEastElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrEastElectrodeTemp.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.LineColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastElectrodeTemp.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;

        }

        /// <summary>
        /// 東短辺温度変化速度
        /// </summary>
        private void CreateChrEastRateTempChange()
        {

            chrEastRateTempChange.BackColor = Color.Black;
            chrEastRateTempChange.Legends.Clear();
            chrEastRateTempChange.Series.Clear();

            // Series
            chrEastRateTempChange.Series.Add("短辺熱電対温度変化速度推定値VTCE_Z");
            chrEastRateTempChange.Series.Add("短辺熱電対温度変化速度推定値VTCE_Z2");
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].ChartType = SeriesChartType.Spline;
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].Color = Color.FromArgb(0, 125, 241);
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z"].BorderWidth = 2;
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].ChartType = SeriesChartType.Spline;
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].Color = Color.FromArgb(241, 139, 0);
            chrEastRateTempChange.Series["短辺熱電対温度変化速度推定値VTCE_Z2"].BorderWidth = 2;

            // Areas
            chrEastRateTempChange.ChartAreas[0].BackColor = Color.Black;
            chrEastRateTempChange.ChartAreas[0].Position = new ElementPosition(6, 0, 93, 100);
            chrEastRateTempChange.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrEastRateTempChange.ChartAreas[0].BorderWidth = 2;
            chrEastRateTempChange.ChartAreas[0].BorderColor = Color.White;

            // X軸
            chrEastRateTempChange.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.Minimum = 40;
            chrEastRateTempChange.ChartAreas[0].AxisX.Maximum = 340;
            chrEastRateTempChange.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrEastRateTempChange.ChartAreas[0].AxisX.Title = "熱電対位置";
            chrEastRateTempChange.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            chrEastRateTempChange.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chrEastRateTempChange.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrEastRateTempChange.ChartAreas[0].AxisX.MinorGrid.Interval = 20;
            chrEastRateTempChange.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrEastRateTempChange.ChartAreas[0].AxisX.LineColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);

            // Y軸
            chrEastRateTempChange.ChartAreas[0].AxisY.Maximum = 2;
            chrEastRateTempChange.ChartAreas[0].AxisY.Minimum = -6;
            chrEastRateTempChange.ChartAreas[0].AxisY.Interval = 1;
            chrEastRateTempChange.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chrEastRateTempChange.ChartAreas[0].AxisY.Title = "温度変化速度（℃/s）";
            chrEastRateTempChange.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular);
            chrEastRateTempChange.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chrEastRateTempChange.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
            chrEastRateTempChange.ChartAreas[0].AxisY.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrEastRateTempChange.ChartAreas[0].AxisY.MinorGrid.Interval = 50;
            chrEastRateTempChange.ChartAreas[0].AxisY.MinorGrid.LineColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrEastRateTempChange.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrEastRateTempChange.ChartAreas[0].AxisY.LineColor = Color.White;
        }

        /// <summary>
        /// 西短辺熱電対温度
        /// </summary>
        private void CreateChrWesternElectrodeTemp()
        {
            chrWesternElectrodeTemp.BackColor = Color.Black;
            chrWesternElectrodeTemp.Legends.Clear();
            chrWesternElectrodeTemp.Series.Clear();

            // Series
            chrWesternElectrodeTemp.Series.Add("短辺熱電対温度推定値TTCW");
            chrWesternElectrodeTemp.Series.Add("短辺熱電対温度推定値TTCW2");
            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].ChartType = SeriesChartType.Spline;
            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].Color = Color.FromArgb(0, 125, 241);
            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW"].BorderWidth = 2;

            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].ChartType = SeriesChartType.Spline;
            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].Color = Color.FromArgb(241, 139, 0);
            chrWesternElectrodeTemp.Series["短辺熱電対温度推定値TTCW2"].BorderWidth = 2;

            // Areas
            chrWesternElectrodeTemp.ChartAreas[0].Position = new ElementPosition(6, 0, 93, 100);
            chrWesternElectrodeTemp.ChartAreas[0].BackColor = Color.Black;
            chrWesternElectrodeTemp.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrWesternElectrodeTemp.ChartAreas[0].BorderWidth = 2;
            chrWesternElectrodeTemp.ChartAreas[0].BorderColor = Color.White;

            // X軸
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.Minimum = 40;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.Maximum = 340;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.Title = "熱電対位置";
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.LineColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.Interval = 20;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrWesternElectrodeTemp.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.White;

            // Y軸
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.Title = "熱電対温度（℃）";
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.LineColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.Maximum = 300;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.Minimum = 50;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.Interval = 50;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chrWesternElectrodeTemp.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
        }

        /// <summary>
        /// 西短辺温度変化速度
        /// </summary>
        private void CreateChrWesternRateTempChange()
        {

            chrWesternRateTempChange.BackColor = Color.Black;
            chrWesternRateTempChange.Legends.Clear();
            chrWesternRateTempChange.Series.Clear();

            // Series
            chrWesternRateTempChange.Series.Add("短辺熱電対温度変化速度推定値VTCW_Z");
            chrWesternRateTempChange.Series.Add("短辺熱電対温度変化速度推定値VTCW_Z2");
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].ChartType = SeriesChartType.Spline;
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].Color = Color.FromArgb(0, 125, 241);
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z"].BorderWidth = 2;
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].ChartType = SeriesChartType.Spline;
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].Color = Color.FromArgb(241, 139, 0);
            chrWesternRateTempChange.Series["短辺熱電対温度変化速度推定値VTCW_Z2"].BorderWidth = 2;

            // Areas
            chrWesternRateTempChange.ChartAreas[0].BackColor = Color.Black;
            chrWesternRateTempChange.ChartAreas[0].Position = new ElementPosition(6, 0, 93, 100);
            chrWesternRateTempChange.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
            chrWesternRateTempChange.ChartAreas[0].BorderWidth = 2;
            chrWesternRateTempChange.ChartAreas[0].BorderColor = Color.White;

            // X軸
            chrWesternRateTempChange.ChartAreas[0].AxisX.Minimum = 40;
            chrWesternRateTempChange.ChartAreas[0].AxisX.Maximum = 340;
            chrWesternRateTempChange.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chrWesternRateTempChange.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chrWesternRateTempChange.ChartAreas[0].AxisX.Title = "熱電対位置";
            chrWesternRateTempChange.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
            chrWesternRateTempChange.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrWesternRateTempChange.ChartAreas[0].AxisX.MinorGrid.Interval = 20;
            chrWesternRateTempChange.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
            chrWesternRateTempChange.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisX.LineColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisX.TitleForeColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;

            // Y軸
            chrWesternRateTempChange.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chrWesternRateTempChange.ChartAreas[0].AxisY.Minimum = -6;
            chrWesternRateTempChange.ChartAreas[0].AxisY.Maximum = 2;
            chrWesternRateTempChange.ChartAreas[0].AxisY.Interval = 1;
            chrWesternRateTempChange.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chrWesternRateTempChange.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chrWesternRateTempChange.ChartAreas[0].AxisY.Title = "温度変化速度（℃/s）";
            chrWesternRateTempChange.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
            chrWesternRateTempChange.ChartAreas[0].AxisY.MinorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chrWesternRateTempChange.ChartAreas[0].AxisY.MinorGrid.Interval = 50;
            chrWesternRateTempChange.ChartAreas[0].AxisY.MinorGrid.LineColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisY.LineColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisY.TitleForeColor = Color.White;
            chrWesternRateTempChange.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("MS Gothic", 8F, System.Drawing.FontStyle.Bold);
        }

        #endregion

        #region イベント処理
        /// <summary>
        /// 表示ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_loadchart_Click(object sender, EventArgs e)
        {
            if (isRealTime == false)
            {
                Mdlres mdlres = new Mdlres();
                DataSet dsMdlresBO = new DataSet();

                long dFromLg = long.Parse(dtpFrom.Value.ToString("yyyyMMddHHmmss"));
                long dTolg = long.Parse(dtpTo.Value.ToString("yyyyMMddHHmmss"));
                float mM = float.Parse(txtmM.Text);

                dsMdlresBO = mdlres.loadDataBO(dFromLg, dTolg, mM);

                LoadPointChrEastBO(dsMdlresBO);
                LoadPointChrWesternBO(dsMdlresBO);
                LoadPointChrEastElectrodeTemp(dFromLg, dTolg, mM);
                LoadPointChrWesternElectrodeTemp(dFromLg, dTolg, mM);
                LoadPointChrEastRateTempChange(dFromLg, dTolg, mM);
                LoadPointChrWesternRateTempChange(dFromLg, dTolg, mM);
            }
            else
            {
                CreateChrEastBO();
                CreateChrWesternBO();
                CreateChrEastElectrodeTemp();
                CreateChrEastRateTempChange();
                CreateChrWesternElectrodeTemp();
                CreateChrWesternRateTempChange();
                MainRealTime();
            }
        }
        /// <summary>
        /// リアルタイム表示チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbRealTime_CheckedChanged(object sender, EventArgs e)
        {
            if (chbRealTime.Checked)
            {
                isRealTime = true;
                dtpFrom.Enabled = false;
                dtpTo.Enabled = false;
                txtmM.Enabled = false;
            }
            else
            {
                isRealTime = false;
                dtpFrom.Enabled = true;
                dtpTo.Enabled = true;
                txtmM.Enabled = true;
            }
        }
        /// <summary>
        /// 警告履歴ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistory_Click(object sender, EventArgs e)
        {
            new HistoryForm().Show();
        }

        #endregion
    }
}

