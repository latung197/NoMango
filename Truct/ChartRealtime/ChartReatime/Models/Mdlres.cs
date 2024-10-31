using System.Data;
namespace ChartRealtime.Models
{
    public class Mdlres
    {
        public Mdlres()
        {
        }
        /// <summary>
        /// BO危険度・警告グラフのデータ取得
        /// </summary>
        /// <param name="dFromLgE">東短辺データ取得した最後時点</param>
        /// <param name="dFromLgW">西短辺データ取得した最後時点</param>
        /// <param name="dTimeTrandMelt">初期凝固遅れ・再溶解取得した最後時点</param>
        /// <returns>BO危険度・警告のグラフデータ</returns>
        public DataSet LoadRealTimeBO(long dFromLgE, long dFromLgW, long dTimeTrandMelt)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("rpt_RealTimeBO");
                sp.AddParam("dFromE", dFromLgE);
                sp.AddParam("dFromW", dFromLgW);
                sp.AddParam("dFromTrent", dTimeTrandMelt);
                returnStruct = DbHelper.ExecuteProc(sp);
                if (returnStruct.ReturnDataSet != null)
                {
                    return returnStruct.ReturnDataSet;
                }
                else
                {
                    return null;
                }
            }
            catch 
            {
                return null;
            }
        }


        /// <summary>
        /// グラフのデータ履歴取得
        /// </summary>
        /// <param name="dFromLg">期間（FROM）</param>
        /// <param name="dTolg">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        /// <returns>BO危険度・警告のグラフデータ</returns>
        public DataSet loadDataBO(long dFromLg, long dTolg, float mM)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("rpt_LoadDataBO");
                sp.AddParam("dFrom", dFromLg);
                sp.AddParam("dTo", dTolg);
                sp.AddParam("mM", mM);
                returnStruct = DbHelper.ExecuteProc(sp);
                return returnStruct.ReturnDataSet;
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
    }
}
