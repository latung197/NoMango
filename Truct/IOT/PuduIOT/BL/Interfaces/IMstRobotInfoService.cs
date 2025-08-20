using PuduIOT.Models;

namespace PuduIOT.BL.Interfaces
{
    public interface IMstRobotInfoService
    {
        List<MstRobotStatus> GetAll();
        List<MstRobotStatus> GetById( int id);
    }
}
