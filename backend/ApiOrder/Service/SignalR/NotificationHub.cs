using Microsoft.AspNetCore.SignalR;

namespace ApiOrder.Service.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("GetListOrder", message);
        }
    }
}
