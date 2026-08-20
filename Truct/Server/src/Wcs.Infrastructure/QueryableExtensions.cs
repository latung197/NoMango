using System.Text.Json.Serialization;

namespace Wcs.Infrastructure;

public interface IPaginationInfo
{ 
    int PageNumber { get; }
    int PageSize { get; }
}

public abstract class QueryArgsBase : IPaginationInfo
{
    [JsonPropertyName("page")]
    public virtual int PageNumber { get; set; } = 1;
    [JsonPropertyName("page_size")]
    public virtual int PageSize { get; set; } = 20;
}

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>( 
        this IQueryable<T> source, 
        int page, int pageSize = 20 )
    {
        return source
            .Skip( ( page - 1 ) * pageSize )
            .Take( pageSize );
    }
}