namespace Core.Domain
{
    public interface IAuditable
    {
        string? CreateTime { get; set; }
        string? CreateId { get; set; }
        string? UpdateTime { get; set; }
        string? UpdateId { get; set; }
    }
}
