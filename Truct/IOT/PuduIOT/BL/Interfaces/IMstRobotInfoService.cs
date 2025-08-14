using PuduIOT.Models;

namespace PuduIOT.BL.Interfaces
{
    public interface IMstRobotInfoService
    {
        List<MstRobotInfor> GetAll();
        List<MstRobotInfor> GetById( int id);
    }
}
