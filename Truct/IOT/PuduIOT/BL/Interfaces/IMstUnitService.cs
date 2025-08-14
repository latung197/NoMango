using PuduIOT.Models;

namespace PuduIOT.BL.Interfaces
{
    public interface IMstUnitService
    {
        List<MstUnit> GetUnitsByType(string unit_type);

        public List<MstUnit> GetUnitsByLocation(string location);

        public void UpdateUnit(int id, decimal value);
    }
}
