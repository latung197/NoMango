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



        private readonly IMstUnitService _unitService;

        private static int _count = 0;

        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static List<(string clientId, CancellationTokenSource cts)> runningTasks = new List<(string, CancellationTokenSource)>();

        private static TaskManager taskManager = new TaskManager();

        public RealTimeHub(PuduIotDbContext context, ILibs libs, IMstUnitService unitService)
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
                    RobotData robotData = null;
                    DataTable dtRobots = new DataTable();
                    List<RobotData> robotDataList = new List<RobotData>();
                    try
                    {
                        dtRobots = _libs.ExecuteFunction("SELECT id, sn, name, company_id, company_name, img_name FROM public.mst_robot_infor;");
                        foreach (DataRow dr in dtRobots.Rows)
                        {
                            string json = await GetDataPudu("/open-platform-service/v1/status/get_by_sn?sn=" + dr["sn"].ToString() + "");
                            if (!string.IsNullOrEmpty(json))
                            {
                                var robotResponse = JsonConvert.DeserializeObject<RobotResponse>(json);

                                if (robotResponse != null && robotResponse.Data != null)
                                {
                                    robotData = robotResponse.Data;
                                    robotData.Sn = dr["sn"].ToString();
                                    robotData.Name = dr["name"].ToString();
                                    robotData.ImagName = dr["img_name"].ToString();
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

    public async Task<string> GetDataPudu(string pathAndQuery)
    {
        string HTTPMethod = "GET";
        string Accept = "application/json";
        string ContentType = "application/json";

        // 应用 ApiAppKey
        string ApiAppKey = "APIDdPWPWY2EVEPTKWFOd5NNkEpuHRSb9FPjn3n8h";
        string ApiAppSecret = "aeipzmPCHWrVthBfbeHDPwnyiNpx32efBB4foCn9Z";

        // string url = "https://css-open-platform.pudutech.com/pudu-entry/data-open-platform-service/v1/api/robot?limit=2&offset=0&shop_id=526150005";

        // Base URL node Mỹ (thay theo khu vực bạn)
        string baseUrl = "https://css-open-platform.pudutech.com/pudu-entry";
        // API healthCheck + query string

        string url = baseUrl + pathAndQuery;

        Uri uri = new Uri(url);
        string host = uri.Host;
        string path = uri.AbsolutePath;
        Console.WriteLine("Url:{0}", url);
        Console.WriteLine("Host:{0}", host);

        // Without environmental information
        if (path.StartsWith("/release"))
        {
            path = path.Substring("/release".Length);

        }
        else if (path.StartsWith("/test"))
        {
            path = path.Substring("/test".Length);
        }
        else if (path.StartsWith("/prepub"))
        {
            path = path.Substring("/prepub".Length);
        }
        if (path == "")
        {
            path = "/";
        }
        //query sort
        if (uri.Query.Length > 0)
        {
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            List<string> lstQuery = new List<string>();
            foreach (var key in queryString.AllKeys)
            {
                lstQuery.Add(key);
            }
            lstQuery.Sort();
            StringBuilder sbQuery = new StringBuilder();
            foreach (string q in lstQuery)
            {
                if (queryString[q] != "")
                {
                    sbQuery = sbQuery.Append("&").Append(q).Append("=").Append(queryString[q]);
                }
                else
                {
                    sbQuery = sbQuery.Append("&").Append(q);

                }
            }
            path += "?" + sbQuery.ToString().TrimStart('&');
        }

        var xDate = DateTime.UtcNow.ToUniversalTime().ToString("r");
        string contentMd5 = "";
        string bodyStr = "{\"b\":\"2\", \"a\":\"###特殊字符测试\", \"c\": \"3\"}";
        if (HTTPMethod == "POST")
        {
            //Content-MD5
            byte[] result = Encoding.UTF8.GetBytes(bodyStr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string hexString = BitConverter.ToString(output).Replace("-", "").ToLower();
            byte[] bs = System.Text.Encoding.ASCII.GetBytes(hexString);
            contentMd5 = Convert.ToBase64String(bs);
        }
        string signingStr = string.Format("x-date: {0}\n{1}\n{2}\n{3}\n{4}\n{5}", xDate, HTTPMethod, Accept, ContentType, contentMd5, path);

        //HMACSHA1
        HMACSHA1 hmacsha1 = new HMACSHA1();
        hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(ApiAppSecret);
        byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(signingStr);
        byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
        string signature = Convert.ToBase64String(hashBytes);

        //get authorization
        string sign = string.Format("hmac id=\"{0}\", algorithm=\"hmac-sha1\", headers=\"x-date\", signature=\"{1}\"", ApiAppKey, signature);
        Console.WriteLine("sign:" + sign);


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = HTTPMethod;
        request.Host = host;
        request.ContentType = ContentType;
        request.Accept = Accept;
        request.Headers.Add("x-date", xDate);
        request.Headers.Add("Authorization", sign);
        request.Headers.Add("Content-MD5", contentMd5);
        try
        {
            if (HTTPMethod == "POST")
            {
                //post request body
                byte[] byteData = Encoding.UTF8.GetBytes(bodyStr);
                int length = byteData.Length;
                request.ContentLength = length;
                Stream writer = request.GetRequestStream();
                writer.Close();
                return null;
            }
            //get response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }

        }
        catch (Exception ex)
        {
            return null;
        }
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