using Microsoft.EntityFrameworkCore;
using PuduIOT.BL.Interfaces;
using PuduIOT.Models;

namespace PuduIOT.DA.Respository
{
    public class MstRobotInfoRepository
    {
        private readonly PuduIotDbContext _context;
        private readonly ILibs _lips;
        public MstRobotInfoRepository(PuduIotDbContext context, ILibs libs)
        {
            _context = context;
            _lips = libs;
        }

        public async Task <List<MstRobotInfor>> GetAllRobot()
        {
            return await _context.MstRobotInfo.ToListAsync();
        }

        public async Task<MstRobotInfor> GetById(string sn)
        {
            return await _context.MstRobotInfo.FirstOrDefaultAsync(x=>x.Sn == sn);
        }

        public async Task<string> ReCharge(string path)
        {
            return await _lips.GetDataPudu(path);
        }

        public async Task<string> GetListPoint(string path)
        {
            return await _lips.GetDataPudu(path);
        }

        public async Task<string> CustomCall(string path, string json)
        {

            string str = await _lips.CallCustom(path, json);
            return str;
        }  
        public async Task<string> CancelTask(string path, string json)
        {
            string str = await _lips.CallCustom(path, json);
            return str;
        }

    }
}
