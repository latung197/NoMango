using NuGet.Protocol.Core.Types;
using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.DA.Respository;
using PuduIOT.Models;
using PuduIOT.Models.Robots.ListPoint;
using System.IO;

namespace PuduIOT.BL.Services
{
    public class MstRobotInfoService : IMstRobotInfoService
    {
        private readonly MstRobotInfoRepository _repository;
        private readonly ILibs _libs;
        public MstRobotInfoService(PuduIotDbContext context, ILibs libs)
        {
            _repository = new MstRobotInfoRepository(context, libs);
        }
        public async Task<List<MstRobotInfor>> GetAll()
        {
            return await _repository.GetAllRobot();
        }

        public async Task<MstRobotInfor> GetById(string id)
        {
            return await _repository.GetById(id);

        }

        public async Task<string> ReCharge(string sn)
        {
            string pathAndQuery = "/open-platform-service/v1/recharge?sn=";
            string url = pathAndQuery + sn;
            return await _repository.ReCharge(url);
        }

        public async Task<List<Points>> GetListPoint(string sn)
        {
            string pathAndQuery = "/map-service/v1/open/point?sn=" + sn + "&limit=100&offset=0";
            string url = pathAndQuery;
            string json = await _repository.GetListPoint(url);

            // Parse JSON sang object
            var result = System.Text.Json.JsonSerializer.Deserialize<PointListResponse>(json);
            return result?.data?.list ?? new List<Points>();

        }

        public async Task<string> CustomCall(string Sn, string mapName, string point, string pointType)
        {
            string url = "https://css-open-platform.pudutech.com/pudu-entry/open-platform-service/v1/custom_call";
            string bodyStr = string.Format(@"
                            {{
                              ""sn"": ""{0}"",
                              ""map_name"": ""{1}"",
                              ""point"": ""{2}"",
                              ""point_type"": ""{3}"",
                              ""call_device_name"": ""APIDdPWPWY2EVEPTKWFOd5NNkEpuHRSb9FPjn3n8h"",
                              ""call_mode"": """"
                            }}", Sn, mapName, point, pointType);
            var result = await _repository.CustomCall(url, bodyStr);
            return result;

        } 
        
        public async Task<string> CancelTask(string taskId)
        {
            string url = "https://css-open-platform.pudutech.com/pudu-entry/open-platform-service/v1/custom_call/cancel";

            string bodyStr = string.Format(@"
                            {{
                              ""task_id"": ""{0}"",
                              ""call_device_name"": ""APIDdPWPWY2EVEPTKWFOd5NNkEpuHRSb9FPjn3n8h""
                            }}", taskId);

            // Parse JSON sang object
            var result = await _repository.CancelTask(url, bodyStr);

            return result;

        }
    }
}
