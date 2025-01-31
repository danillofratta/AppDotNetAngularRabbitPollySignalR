using Microsoft.AspNetCore.SignalR;

namespace ApiSale.Service.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("GetListSale", message);
        }
    }
}
