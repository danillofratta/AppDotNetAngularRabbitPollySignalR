using Microsoft.AspNetCore.SignalR;

namespace ApiStock.Service.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("GetListStock", message);
        }
    }
}
