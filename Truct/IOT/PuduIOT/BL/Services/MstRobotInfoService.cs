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
        public MstRobotInfoService(PuduIotDbContext context)
        {
            _repository = new MstRobotInfoRepository(context);
        }
        public async Task<List<MstRobotInfor>> GetAll()
        {
            return await _repository.GetAllRobot();
        }

        public async Task<MstRobotInfor> GetById(string id)
        {
            return await _repository.GetById(id);

        }
    }
}
