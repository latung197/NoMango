using System.Data;

namespace PuduIOT.Models
{
    public class MyData
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string[] html { get; set; }
        public string timeType { get; set; }
        public string unitCode { get; set; }

        public int eDashboard { get; set; }
        public List<DataChart> data { get; set; }

    }

    public class DataChart
    {
        public string datetime { get; set; }
        public double totalvalue { get; set; }
    }

    public class StatisticalTable
    {
        public int id { get; set; }
        public string item_code { get; set; }
        public double unit_value { get; set; }
        public double standard_value { get; set; }
        public double performance { get; set; }
    }


    public class MyDataModel
    {
        public DataTable DataNow { get; set; }
        public DataTable DayChart { get; set; }
        public DataTable MonthChart { get; set; }
        public DataTable YearChart { get; set; }
       
        public List<DataChart> DataChart { get; set; }
        public List<StatisticalTable> StatisticalTable { get; set; }
    }
}
