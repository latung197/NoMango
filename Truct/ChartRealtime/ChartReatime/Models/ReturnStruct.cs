using System.Data;

namespace ChartRealtime.Models
{

    public struct ReturnStruct
    {

        public DataSet ReturnDataSet
        {
            set;
            get;
        }

        public string ReturnMessage
        {
            set;
            get;
        }
    }
}
