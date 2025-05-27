
namespace Core.Application.Interface
{
    public interface IPagging
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
    }
}
