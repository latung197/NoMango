using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Model
{
    public class GenericResponseResult<T>
    {
        [JsonProperty("pageIndex")]
        [Description("index trang tìm kiếm")]
        public int PageIndex { get; set; }

        [JsonProperty("pageSize")]
        [Description("số bản ghi mỗi trang")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        [Description("Tổng số trang kết quả")]
        public int TotalPages { get; set; }

        [JsonProperty("totalFilter")]
        [Description("Tổng số bản ghi kết quả")]
        public int TotalFilter { get; set; }

        [JsonProperty("listData")]
        [Description("Danh sách kết quả trả về")]
        public List<T> ListData { get; set; }

        [JsonProperty("message")]
        [Description("Message phản hồi xử lý")]
        public string Message { get; set; }
    }

}
