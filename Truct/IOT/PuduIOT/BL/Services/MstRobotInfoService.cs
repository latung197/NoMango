using NuGet.Protocol.Core.Types;
using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.DA.Respository;
using PuduIOT.Models;

namespace PuduIOT.BL.Services
{
    public class MstRobotInfoService : IMstRobotInfoService
    {
        private readonly MstRobotInfoRepository _repository;
        public MstRobotInfoService(HamadenDbContext context)
        {
            _repository = new MstRobotInfoRepository(context);
        }
        public List<MstRobotStatus> GetAll()
        {
            return _repository.GetAllRobot();
        }

        public List<MstRobotStatus> GetById(int id)
        {
            return _repository.GetById(id);

        }
    }
}
