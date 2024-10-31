using System.Data;
using System.Data.SqlClient;

namespace ChartRealtime.Models
{
    /// <summary>
    /// プロシージャ処理クラス
    /// </summary>
    public class SqlParam
    {
        /// <summary>
        /// プロシージャ名
        /// </summary>
        public string sProcName = "";

        /// <summary>
        /// プロシージャのパラメータ
        /// </summary>
        private DataTable dt = new DataTable();
        public SqlParam(string sProcName)
        {
            this.sProcName = sProcName;

            dt.Columns.Add("param");
            dt.Columns.Add("value");
            dt.Columns["param"].DataType = typeof(string);
            dt.Columns["value"].DataType = typeof(object);
        }

        /// <summary>
        /// パラメータ追加
        /// </summary>
        /// <param name="sParam">パラメータ名</param>
        /// <param name="sValue">パラメータ値</param>
        public void AddParam(string sParam, object sValue)
        {
            if (sProcName == "")
                return;
            DataRow row = dt.NewRow();
            row["param"] = sParam;
            row["value"] = sValue;
            dt.Rows.Add(row);
        }

        /// <summary>
        /// パラメータ取得
        /// </summary>
        /// <returns>パラメータ配列</returns>
        public SqlParameter[] SetSqlParameter()
        {
            SqlParameter[] SqlParameterList = new SqlParameter[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SqlParameter SqlParameter = new SqlParameter(dt.Rows[i][0].ToString(), dt.Rows[i][1]);
                SqlParameterList[i] = SqlParameter;
            }
            return SqlParameterList;
        }
    }
}
