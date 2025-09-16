namespace PuduIOT.BL.Interfaces
{
    public interface IPuduApiService
    {
        Task<string> GetDataAsync(string url);
        Task<string> ReCharge(string sn);
    }
}
