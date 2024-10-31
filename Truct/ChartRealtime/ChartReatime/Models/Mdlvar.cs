using System;
using System.Data;

namespace ChartRealtime.Models
{
    public class Mdlvar
    {
        public Mdlvar()
        {
        }
        /// <summary>
        /// 熱電対温度グラフデータリアタイム取得
        /// </summary>
        /// <param name="dFromLg">取得した最後時点</param>
        /// <param name="eW">'E' 東短辺 || 'W' 西短辺</param>
        /// <returns>熱電対温度グラフデータ</returns>
        public DataSet LoadRealTimeTemp(long dFromLgN,long dFromLgO, string eW)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("[rpt_RealTimeTemp]");
                sp.AddParam("dFromN", dFromLgN);
                sp.AddParam("dFromO", dFromLgO);
                sp.AddParam("EW", eW);
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
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 温度変化速度グラフデータリアタイム取得
        /// </summary>
        /// <param name="dFromN">取得した最後時点</param>
        /// <param name="dFromO">取得した最後時点</param>
        /// <param name="eW">'E' 東短辺 || 'W' 西短辺</param>
        /// <returns>温度変化速度グラフデータ</returns>
        public DataSet LoadRealTimeTempChange(long dFromN,long dFromO, string eW)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("[rpt_RealTimeTempChange]");
                sp.AddParam("dFromN", dFromN);
                sp.AddParam("dFromO", dFromO);
                sp.AddParam("EW", eW);
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
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 熱電対温度グラフデータ取得
        /// </summary>
        /// <param name="dFromLg">期間（FROM）</param>
        /// <param name="dToLg">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        /// <param name="eW">'E' 東短辺 || 'W' 西短辺</param>
        /// <returns>熱電対温度グラフデータ</returns>
        public DataSet LoadTemp(long dFromLg, long dToLg, float mM, string eW)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("[rpt_LoadTemp]");
                sp.AddParam("dFrom", dFromLg);
                sp.AddParam("dTo", dToLg);
                sp.AddParam("mM", mM);
                sp.AddParam("EW", eW);
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
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 温度変化速度グラフデータ取得
        /// </summary>
        /// <param name="dFromLg">期間（FROM）</param>
        /// <param name="dToLg">期間（TO）</param>
        /// <param name="mM">鋳造長</param>
        /// <param name="eW">'E' 東短辺 || 'W' 西短辺</param>
        /// <returns>温度変化速度グラフデータ</returns>
        public DataSet LoadRateTempChange(long dFromLg, long dToLg, float mM, string eW)
        {
            try
            {
                ReturnStruct returnStruct = new ReturnStruct();
                SqlParam sp = new SqlParam("[rpt_LoadRateTempChange]");
                sp.AddParam("dFrom", dFromLg);
                sp.AddParam("dTo", dToLg);
                sp.AddParam("mM", mM);
                sp.AddParam("EW", eW);
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

    }
}
