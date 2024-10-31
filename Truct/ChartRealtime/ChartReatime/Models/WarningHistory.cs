using System;
using System.Data;

namespace ChartRealtime.Models
{
    public class WarningHistory
    {
        public WarningHistory() 
        { 
        }
        /// <summary>
        /// 警告履歴取得
        /// </summary>
        /// <param name="dFrom">期間（FROM）</param>
        /// <param name="dTo">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        /// <param name="PageIndex">ページネーションインデックス</param>
        /// <param name="PageSize">ページ内に表示件数</param>
        /// <returns>警告データと総件数</returns>
        public DataSet LoadHistory(DateTime dFrom, DateTime dTo, float mM, float FromWarningPoint, float ToWarningPoint, int PageIndex, int PageSize)
        {

            long dFromLg = long.Parse(dFrom.ToString("yyyyMMddHHmmss"));
            long dTolg = long.Parse(dTo.ToString("yyyyMMddHHmmss"));

            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("rpt_HistoryMDLRES");
                sp.AddParam("@dFrom", dFromLg);
                sp.AddParam("@dTo", dTolg);
                sp.AddParam("@mM", mM);
                sp.AddParam("@FromWarningPoint", FromWarningPoint);
                sp.AddParam("@ToWarningPoint", ToWarningPoint);
                sp.AddParam("@PageIndex", PageIndex);
                sp.AddParam("@PageSize", PageSize);
                returnStruct = DbHelper.ExecuteProc(sp);
                if(returnStruct.ReturnDataSet!=null)
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

    }
}
