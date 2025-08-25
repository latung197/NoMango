using PuduIOT.Models;

namespace PuduIOT.BL.Interfaces
{
    public interface IMstRobotInfoService
    {
        Task<List<MstRobotInfor>> GetAll();
        Task<MstRobotInfor> GetById( string id);
    }
}
