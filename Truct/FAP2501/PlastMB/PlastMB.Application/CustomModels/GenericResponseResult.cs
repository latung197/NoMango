using System.ComponentModel;

namespace PlastMB.Application.CustomModels
{
    [Description("Kết quả phản hồi tìm kiếm API")]
    public class GenericResponseResult<T>
    {
        #region Properties
        //Current page
        public int PageIndex { get; set; }
        //Total paging
        public int TotalPages { get; set; }
        //Number record per page
        public int PageSize { get; set; }
        //Total record
        public int TotalFilter { get; set; }
        //List Data
        public List<T> ListData { get; set; }
        public string Message { get; set; }
        #endregion
        #region Constructor
        public GenericResponseResult(List<T> items, int count, int pageIndex, int pageSize, string mesage = "")
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalFilter = count;
            ListData = items;
            Message = mesage;
        }

        public GenericResponseResult(List<T> items)
        {
            PageIndex = 1;
            TotalPages = 1;
            PageSize = items.Count;
            TotalFilter = items.Count;
            ListData = items;
        }
        public GenericResponseResult()
        {
            PageIndex = 0;
            TotalPages = 1;
            PageSize = 0;
            TotalFilter = 0;
            ListData = null;
        }
        public GenericResponseResult(string mesage)
        {
            PageIndex = 0;
            TotalPages = 1;
            PageSize = 0;
            TotalFilter = 0;
            ListData = null;
            Message = mesage;
        }
        #endregion
    }
}
