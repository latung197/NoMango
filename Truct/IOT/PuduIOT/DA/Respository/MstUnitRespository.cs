using PuduIOT.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuduIOT.DA.Respository
{

    public class MstUnitRespository
    {
        private readonly HamadenDbContext _context;

        public MstUnitRespository(HamadenDbContext context)
        {
            _context = context;
        }

        public List<MstUnit> GetUnitsByType(string unit_type)
        {
            return _context.MstUnits.Where(u => u.unit_type == unit_type).OrderBy(u => u.location).ThenBy(u => u.id).ToList();
        }

        public List<MstUnit> GetUnitsByLocation(string location)
        {
            return _context.MstUnits.Where(u => u.location == location).ToList();
        }

        public void UpdateStandardValueByUnit(int id, decimal value)
        {
            var existingUnit = _context.MstUnits.Find(id);
            if (existingUnit != null)
            {
                existingUnit.standard_value = value;
                existingUnit.update_time = DateTime.UtcNow;
                existingUnit.update_id = "admin";
                _context.SaveChanges();
            }
        }
    }
}
