namespace PuduIOT.BL.Interfaces
{
    public interface IPuduApiService
    {
       public Task<string> GetDataAsync(string url);
    }
}
