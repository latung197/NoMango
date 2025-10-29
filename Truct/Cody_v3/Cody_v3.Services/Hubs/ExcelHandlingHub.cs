using Microsoft.AspNetCore.SignalR;

namespace Cody_v3.Services.Hubs
{
    public class ExcelHandlingHub: Hub<IExcelHandlingClient>
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("====================\nOnConnectedAsync: {0} {1}", Context.ConnectionId, DateTime.Now);
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("====================\nOnDisconnectedAsync: {0} {1}", Context.ConnectionId, DateTime.Now);
            await base.OnDisconnectedAsync(exception);
        }


        public async Task ReceiveMessage(string message, double percent)
        {
            await Clients.Caller.ReceiveMessage(message, percent);
        }

        //public async Task SendMessageToAllExpectSelf(string message)
        //{
        //    await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(message);
        //}

        //public async Task SendMessageToAll(string message)
        //{
        //    await Clients.All.ReceiveMessage(message);
        //}
        //public async Task SendMessage(string user, string message)
        //=> await Clients.All.SendMessageToAll(user, message);

        //public async Task SendMessageToCaller(string user, string message)
        //    => await Clients.Caller.ReceiveMessage(user, message);

        //public async Task SendMessageToGroup(string user, string message)
        //    => await Clients.Group("SignalR Users").ReceiveMessage(user, message);
    }
}
