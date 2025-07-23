using CoreMVC.Models;

namespace CoreMVC.DA.Respository
{
    public class MstUnitRespository
    {
        private readonly CoreDbContext _context;
        public MstUnitRespository(CoreDbContext context)
        {
            _context = context;
        }

        public List<MstUnit> GetUnitByType( string unit_type) 
        {
            return _context.mstUnits.Where(u=>u.unit_type == unit_type).OrderBy(u=>u.location).ThenBy(u=>u.unit_code).ToList();
        }

        public List<MstUnit> GetUnitByMachineId(string machineId)
        {
            return _context.mstUnits.Where(u=>u.machineId==machineId).OrderBy(u => u.location).ThenBy(u => u.unit_code).ToList();
        }
        public List<MstUnit> GetAll()
        {
            return _context.mstUnits.ToList();
        }

        public void UpdateStandardValueByUnit(int id, decimal value)
        {
            var unit = _context.mstUnits.Find(id);
            if(unit != null)
            {
                unit.standard_value = value;
                unit.update_time = DateTime.UtcNow;
                unit.update_id = "admin";
                _context.SaveChanges();
            }
        }
    }
}
