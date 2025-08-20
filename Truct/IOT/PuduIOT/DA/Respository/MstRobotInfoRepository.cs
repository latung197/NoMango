using Microsoft.EntityFrameworkCore;
using PuduIOT.Models;

namespace PuduIOT.DA.Respository
{
    public class MstRobotInfoRepository
    {
        private readonly HamadenDbContext _context;

        public MstRobotInfoRepository(HamadenDbContext context)
        {
            _context = context;
        }

        public List<MstRobotStatus> GetAllRobot()
        {
            return _context.MstRobotInfo.ToList();
        }

        public List<MstRobotStatus> GetById(int id)
        {
            return _context.MstRobotInfo.Where(x=>x.Id == id).ToList();
        }

    }
}
