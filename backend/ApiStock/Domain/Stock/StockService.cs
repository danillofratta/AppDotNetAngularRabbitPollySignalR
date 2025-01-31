using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Models;
using SharedRabbitMq.Service;

namespace ApiStock.Domain.Stock
{
    public class StockService : IDisposable
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;

        public StockService(RabbitMqService rabbitMqService, DBDevContext context)
        {
            _rabbitMqService = rabbitMqService;            
            _context = context;            
        }

        public async Task CheckAvailability(Order order)
        {
            await this.Check(order);
        }

        private async Task Check(Order order)
        {
            SharedDatabase.Models.Stock stock = await _context.Stock.SingleOrDefaultAsync(x => x.Idproduct == order.Idproduct);
            if (stock != null && stock.Amount >= order.Amount)
            {
                await this.NotifyOrderOk(order);
                await this.NotifySaleOk(order);
            }
            else
            {
                await this. NotifyOrderFailed(order);
            }
        }
        private async Task NotifySaleOk(Order order)
        {
            _rabbitMqService._queueName = "StockToSale-StockOK-Queue";
            await _rabbitMqService.InitializeService();
            await _rabbitMqService.SendMessage(order);
        }

        private async Task NotifyOrderOk(Order order)
        {
            _rabbitMqService._queueName = "StockToOrder-StockOK-Queue";
            await _rabbitMqService.InitializeService();
            await _rabbitMqService.SendMessage(order);
        }

        private async Task NotifyOrderFailed(Order order)
        {
            _rabbitMqService._queueName = "StockToOrder-StockFailed-Queue";
            await _rabbitMqService.InitializeService();
            await _rabbitMqService.SendMessage(order);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
