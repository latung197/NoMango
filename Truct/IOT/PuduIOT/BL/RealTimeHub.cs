// HamadenFIot.Hubs.ChartHub
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using PuduIOT.BL.Commons;
using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol;

namespace PuduIOT.BL
{
    public class RealTimeHub : Hub
    {

        private ILibs _libs;



        private readonly IMstUnitService _unitService;

        private static int _count = 0;

        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static List<(string clientId, CancellationTokenSource cts)> runningTasks = new List<(string, CancellationTokenSource)>();

        private static TaskManager taskManager = new TaskManager();

        public RealTimeHub(HamadenDbContext context, ILibs libs, IMstUnitService unitService)
        {
            _libs = libs;
            _unitService = unitService;
        }

        public async Task SendRealTimeData()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    DataTable dtLine = new DataTable();
                    DataTable dtLineSum = new DataTable();
                    DataTable dtPacSum = new DataTable();
                    DataTable dtAirSum = new DataTable();
                    DataTable dtPac = new DataTable();
                    DataTable dtAir = new DataTable();
                    DataTable dtPacPerformance = new DataTable();
                    DataTable dtAirPerformance = new DataTable();
                    DataTable dtLinePerformance = new DataTable();
                    try
                    {
                        var unitsAirConditioner = _unitService.GetUnitsByType("1");
                        var unitsElectric = _unitService.GetUnitsByType("3");
                        var unitsAirCompressor = _unitService.GetUnitsByType("2");
                        string[] rsElectric = _libs.ProcessingParam(unitsElectric);
                        string[] rsAir = _libs.ProcessingParam(unitsAirCompressor);
                        string[] strUnitsPac = _libs.ProcessingParam(unitsAirConditioner);

                        DataTable dtCurrentData = _libs.callFuncPostgre("get_current_day_data_consumption_by_location", new string[] { "units_electric", "units_electric_name", "units_air", "units_air_name", "units_pac", "units_pac_name" }, new object[] { rsElectric[0], rsElectric[1], rsAir[0], rsAir[1], strUnitsPac[0], strUnitsPac[1] });

                        for (int i = 0; i < dtCurrentData.Rows.Count; i++)
                        {
                            map.Add(dtCurrentData.Rows[i]["location"].ToString(), Convert.ToDecimal(dtCurrentData.Rows[i]["unit_value"]).ToString());
                        }
                        decimal total = Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"]) + Convert.ToDecimal(map["Pac"]) + Convert.ToDecimal(map["Air"]);
                        map.Add("total", total.ToString());
                        map.Add("avg", (total / DateTime.Now.Hour).ToString());
                        map.Add("totalLine", (Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"])).ToString());
                        map.Add("avgPac", (Convert.ToDecimal(map["Pac"]) / DateTime.Now.Hour).ToString());
                        map.Add("avgAir", (Convert.ToDecimal(map["Air"]) / DateTime.Now.Hour).ToString());
                        map.Add("avgLine", ((Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"])) / DateTime.Now.Hour).ToString());

                        dtLine = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.ELECTRIC_CABINET_REAL_TIME, rsElectric[0], rsElectric[1], "3" });
                        dtLinePerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.ELECTRIC_CABINET_REAL_TIME, rsElectric[0], rsElectric[1], "3" });
                        dtPac = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.CONDITIONER_REAL_TIME, strUnitsPac[0], strUnitsPac[1], "1" });
                        dtPacPerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.CONDITIONER_REAL_TIME, strUnitsPac[0], strUnitsPac[1], "1" });
                        dtAir = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.COMPRESSOR_REAL_TIME, rsAir[0], rsAir[1], "2" });
                        dtAirPerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.COMPRESSOR_REAL_TIME, rsAir[0], rsAir[1], "2" });
                        dtPacSum = _libs.SumData(dtPac, dtPacPerformance);
                        dtAirSum = _libs.SumData(dtAir, dtAirPerformance);
                        dtLineSum = _libs.SumData(dtLine, dtLinePerformance);

                    }
                    catch (Exception ex)
                    {
                    }

                    await ClientProxyExtensions.SendAsync(arg1: new
                    {
                        Line = dtLineSum,
                        Pac = dtPacSum,
                        Air = dtAirSum,
                        Data = map

                    }.ToJson(), clientProxy: Clients.All, method: "ReceiveRealTimeData");

                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                taskManager.RemoveTask(Context.ConnectionId);
            }

        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _count++;
            if (_count == 1)
            {
                await Task.WhenAll(SendRealTimeData());
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _count--;
            await base.OnDisconnectedAsync(exception);
        }

    }

    public class TaskManager
    {
        private List<(string clientId, CancellationTokenSource cts)> runningTasks = new List<(string, CancellationTokenSource)>();

        public void AddTask(string clientId, CancellationTokenSource cts)
        {
            runningTasks.Add((clientId, cts));
        }

        public void RemoveTask(string clientId)
        {
            var task = runningTasks.FirstOrDefault(t => t.clientId == clientId);

            if (task.cts != null)
            {
                task.cts.Cancel();
            }

            runningTasks.RemoveAll(t => t.clientId == clientId);
        }
    }
}