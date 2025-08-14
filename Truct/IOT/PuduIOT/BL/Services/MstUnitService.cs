
using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.DA.Respository;
using PuduIOT.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace PuduIOT.BL.Services
{
    public class MstUnitService : IMstUnitService
    {
        private readonly MstUnitRespository _repository;

        public MstUnitService(HamadenDbContext context)
        {
            _repository = new MstUnitRespository(context);
        }

        public List<MstUnit> GetUnitsByType(string unit_type)
        {
            return _repository.GetUnitsByType(unit_type);
        }
        public List<MstUnit> GetUnitsByLocation(string location)
        {
            return _repository.GetUnitsByLocation(location);
        }

        public void UpdateUnit(int id, decimal value)
        {
            _repository.UpdateStandardValueByUnit( id,  value);
        }
    }
}
