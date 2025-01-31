using ApiStock.Controller;
using ApiStock.Domain.Stock;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ApiStock.Service
{
    public class ConsumerStockAvailableService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private StockController _controller;

        public ConsumerStockAvailableService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory)
        {
            _rabbitMqService = rabbitMqService;            
            _scopeFactory = scopeFactory;
            //_rabbitMqService.InitializeService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "OrderToStock-CheckProductAvailable-Queue";
            await _rabbitMqService.InitializeService();

            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

            await _rabbitMqService.ReceiveMessages(async (message) =>
            {
                var order = JsonSerializer.Deserialize<Order>(message)!;

                using (StockService service = new StockService(_rabbitMqService, dbContext))
                {
                    await service.CheckAvailability(order);
                }
            });

        }
    }

}
