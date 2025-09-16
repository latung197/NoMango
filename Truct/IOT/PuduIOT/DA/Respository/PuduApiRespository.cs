using PuduIOT.BL.Interfaces;

namespace PuduIOT.DA.Respository
{

    public class PuduApiRespository
    {
        private ILibs _libs;
        public PuduApiRespository(ILibs libs)
        {
            _libs = libs;
        }
        public async Task<string> ReCharge(string path)
        {
            return await _libs.GetDataPudu(path);
            
        }
    }
}
