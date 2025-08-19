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

                    DataTable dtRobots = new DataTable();
                    try
                    {
                        //var unitsAirConditioner = _unitService.GetUnitsByType("1");
                        //var unitsElectric = _unitService.GetUnitsByType("3");
                        //var unitsAirCompressor = _unitService.GetUnitsByType("2");
                        //string[] rsElectric = _libs.ProcessingParam(unitsElectric);
                        //string[] rsAir = _libs.ProcessingParam(unitsAirCompressor);
                        //string[] strUnitsPac = _libs.ProcessingParam(unitsAirConditioner);

                        //DataTable dtCurrentData = _libs.callFuncPostgre("get_current_day_data_consumption_by_location", new string[] { "units_electric", "units_electric_name", "units_air", "units_air_name", "units_pac", "units_pac_name" }, new object[] { rsElectric[0], rsElectric[1], rsAir[0], rsAir[1], strUnitsPac[0], strUnitsPac[1] });

                        //for (int i = 0; i < dtCurrentData.Rows.Count; i++)
                        //{
                        //    map.Add(dtCurrentData.Rows[i]["location"].ToString(), Convert.ToDecimal(dtCurrentData.Rows[i]["unit_value"]).ToString());
                        //}
                        //decimal total = Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"]) + Convert.ToDecimal(map["Pac"]) + Convert.ToDecimal(map["Air"]);
                        //map.Add("total", total.ToString());
                        //map.Add("avg", (total / DateTime.Now.Hour).ToString());
                        //map.Add("totalLine", (Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"])).ToString());
                        //map.Add("avgPac", (Convert.ToDecimal(map["Pac"]) / DateTime.Now.Hour).ToString());
                        //map.Add("avgAir", (Convert.ToDecimal(map["Air"]) / DateTime.Now.Hour).ToString());
                        //map.Add("avgLine", ((Convert.ToDecimal(map["A"]) + Convert.ToDecimal(map["B"]) + Convert.ToDecimal(map["C"])) / DateTime.Now.Hour).ToString());

                        //dtLine = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.ELECTRIC_CABINET_REAL_TIME, rsElectric[0], rsElectric[1], "3" });
                        //dtLinePerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.ELECTRIC_CABINET_REAL_TIME, rsElectric[0], rsElectric[1], "3" });
                        //dtPac = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.CONDITIONER_REAL_TIME, strUnitsPac[0], strUnitsPac[1], "1" });
                        //dtPacPerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.CONDITIONER_REAL_TIME, strUnitsPac[0], strUnitsPac[1], "1" });
                        //dtAir = _libs.callFuncPostgre("get_data_real_time", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.COMPRESSOR_REAL_TIME, rsAir[0], rsAir[1], "2" });
                        //dtAirPerformance = _libs.callFuncPostgre("get_data_real_time_one_hour_prev", new string[] { "pr_table_name", "pr_units", "pr_units_name", "pr_unit_type" }, new object[] { Constants.COMPRESSOR_REAL_TIME, rsAir[0], rsAir[1], "2" });
                        //dtPacSum = _libs.SumData(dtPac, dtPacPerformance);
                        //dtAirSum = _libs.SumData(dtAir, dtAirPerformance);
                        //dtLineSum = _libs.SumData(dtLine, dtLinePerformance);
                        string a = await GetDataPudu("/open-platform-service/v1/status/get_by_sn?sn=826085513060001");

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
            //string pathAndQuery = "/data-open-platform-service/v1/api/map?shop_id=526150005&map_name=0#0#fstv3&device_width=1200&device_height=800";
            //string pathAndQuery = "/data-board/v1/analysis/task/delivery/paging?timezone_offset=8&start_time=1755168000&end_time=1755254399&shop_id=526150005&time_unit=day&group_by=robot";
           // pathAndQuery = "/open-platform-service/v1/status/get_by_sn?sn=826085513060001";

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
                Console.WriteLine("Error:{0}", ex.Message);
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