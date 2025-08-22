using Microsoft.EntityFrameworkCore;
using PuduIOT.Models;

namespace PuduIOT.DA.Respository
{
    public class MstRobotInfoRepository
    {
        private readonly PuduIotDbContext _context;

        public MstRobotInfoRepository(PuduIotDbContext context)
        {
            _context = context;
        }

        public async Task <List<MstRobotInfor>> GetAllRobot()
        {
            return await _context.MstRobotInfo.ToListAsync();
        }

        public async Task<List<MstRobotInfor>> GetById(string sn)
        {
            return await _context.MstRobotInfo.Where(x=>x.Sn == sn).ToListAsync();
        }

    }
}
