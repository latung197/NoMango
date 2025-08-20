
using PuduIOT.Models;
using System.Data;
using static PuduIOT.BL.Commons.Constants;

namespace PuduIOT.BL.Interfaces
{
    public interface ILibs
    {

        DataTable callFuncPostgre(string func_name, string[] paramName, object[] paramValue);

        DataTable callFuncPostgre(string func_name);

        DataTable ExecuteFunction(string func_name);

        string[] ProcessingParam(List<MstUnit> units);

        public byte[] ExportDataTableToPdf(DataTable dataTable, MyData mData, string langcode);

        public DataTable ConvertListToDataTable<T>(List<T> list);

        public string[] getStringTimeType(string timeType, string langcode);

        public string ConvertUnitToFormatQuery(string units);

        public byte[] ExportDataTableToCsv(DataTable dataTable, string timeType);

        public DateTime[] GenerateDateTimeByTimeType(string timeType);

        public byte[] ExportPdfApi(MyData mdata);

        public byte[] ExportCsvApi(MyData mdata);

        public DataTable SearchDataApi(int type, string units, DateTime startTime, DateTime endTime, int eDashboard);
        public string LoadDataApi(string units, int eDashboard);

        public DataTable SumData(DataTable dt, DataTable dtPerformance);

        public DataTable SumData(DataTable dt);

        public DataTable RoundDownTime(DataTable dt, string columnName);
    }
}
