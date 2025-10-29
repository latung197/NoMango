namespace Cody_v3.Services.Hubs
{
    public interface IExcelHandlingClient
    {
        Task ReceiveMessage(string message, double percent);
    }
}
