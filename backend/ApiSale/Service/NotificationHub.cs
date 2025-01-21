using Microsoft.AspNetCore.SignalR;

namespace ApiOrder.Service
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("GetListSale", message);
        }
    }
}
