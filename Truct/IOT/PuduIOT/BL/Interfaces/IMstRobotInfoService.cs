using PuduIOT.Models;
using PuduIOT.Models.Robots.ListPoint;
using System.Drawing;

namespace PuduIOT.BL.Interfaces
{
    public interface IMstRobotInfoService
    {
        Task<List<MstRobotInfor>> GetAll();
        Task<MstRobotInfor> GetById( string id);
        Task<string> ReCharge( string sn );
        Task <List<Models.Robots.ListPoint.Points>> GetListPoint(string sn );
        public Task<string> CustomCall(string Sn, string mapName, string point, string pointType);
        public Task<string> CancelTask(string taskId);
    }
}
