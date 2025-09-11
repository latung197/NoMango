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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using PuduIOT.Models.RobotsT300;
using PuduIOT.Models;
using System.Net.Http;

namespace PuduIOT.BL
{
    public class RealTimeHub : Hub
    {

        private ILibs _libs;

        private static int _count = 0;

        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static List<(string clientId, CancellationTokenSource cts)> runningTasks = new List<(string, CancellationTokenSource)>();

        private static TaskManager taskManager = new TaskManager();

        public RealTimeHub(PuduIotDbContext context, ILibs libs)
        {
            _libs = libs;
        }

        public async Task SendRealTimeData()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    RobotData robotData = null;
                    DataTable dtRobots = new DataTable();
                    List<RobotData> robotDataList = new List<RobotData>();
                    try
                    {
                        dtRobots = _libs.ExecuteFunction("SELECT id, sn, name, company_id, company_name, img_name FROM public.mst_robot_infor;");
                        foreach (DataRow dr in dtRobots.Rows)
                        {
                            string json = await _libs.GetDataPudu("/open-platform-service/v1/status/get_by_sn?sn=" + dr["sn"].ToString() + "");
                            if (!string.IsNullOrEmpty(json))
                            {
                                var robotResponse = JsonConvert.DeserializeObject<RobotResponse>(json);

                                if (robotResponse != null && robotResponse.Data != null)
                                {
                                    robotData = robotResponse.Data;
                                    robotData.Sn = dr["sn"].ToString();
                                    robotData.Name = dr["name"].ToString();
                                    robotData.ImagName = dr["img_name"].ToString();
                                    robotData.Company_id = dr["company_id"].ToString();
                                    robotDataList.Add(robotData);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    await Clients.All.SendAsync(
                        "ReceiveRealTimeData",
                        new
                        {
                            RobotsStatus = robotDataList
                        }.ToJson()
                    );

                    await Task.Delay(5000);
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
        private static readonly Dictionary<string, CancellationTokenSource> _robotTokens = new();
       

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