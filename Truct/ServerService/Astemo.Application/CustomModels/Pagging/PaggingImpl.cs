
using Core.Application.Interface;

namespace Core.Application.CustomModels.Pagging
{
    public class PaggingImpl : IPagging
    {
        //Current page result
        public int PageIndex { get; set; } = 0;
        //Number record per page
        public int PageSize { get; set; } = 30;
    }
}
