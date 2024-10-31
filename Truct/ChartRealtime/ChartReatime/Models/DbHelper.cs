using ChartRealtime.Resources;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ChartRealtime.Models
{
    public class DbHelper
    {

        public static string DBConnString { get; private set; }
        /// <summary>
        /// 初期
        /// </summary>
        public DbHelper()
        {
        }

        /// <summary>
        /// クエリを実行する
        /// </summary>
        /// <param name="Query">クエリ</param>
        /// <returns>クエリの取得データ</returns>
        public static DataTable Adapter(string Query)
        {
            DBConnString = ConfigurationManager.AppSettings.Get("DbconnSQL");

            using (SqlConnection conn = new SqlConnection(DBConnString))
            {
                conn.Open();
                try
                {
                    if (Query.Length <= 0 || string.IsNullOrEmpty(Query))
                        return null;
                    else
                    {
                        using (DataTable table = new DataTable())
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(Query, conn))
                            {
                                adapter.Fill(table);
                            }
                            return table;
                        }

                    }
                }
                catch 
                {
                    MessageBox.Show(MessageError.ErrorConnect);
                    return null;
                }
            }

        }
        
        /// <summary>
        /// プロシージャを呼び出す
        /// </summary>
        /// <param name="sp">プロシージャ名及びプロシージャのパラメータ</param>
        /// <returns>プロシージャ実行結果と取得したデータ</returns>
        public static ReturnStruct ExecuteProc(SqlParam sp)
        {
            ReturnStruct ret = new ReturnStruct();
            DBConnString = ConfigurationManager.AppSettings.Get("DbconnSQL");
            using (SqlConnection conn = new SqlConnection(DBConnString))
            {
                try
                {

                    conn.Open();
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(sp.sProcName, conn))
                            {
                                cmd.CommandTimeout = 60;
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Gán Param
                                foreach (SqlParameter sqlParameter in sp.SetSqlParameter())
                                {
                                    cmd.Parameters.Add(sqlParameter);
                                }

                                SqlDataAdapter sqladp = new SqlDataAdapter(cmd);
                                sqladp.Fill(ds);
                                ret.ReturnDataSet = ds;
                                ret.ReturnMessage = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(MessageError.ErrorSqlString);
                            ret.ReturnMessage = ex.Message;
                        }
                        return ret;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(MessageError.ErrorConnect);
                    ret.ReturnMessage = ex.Message;
                    return ret;
                }
            }
        }

    }
}
