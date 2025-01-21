using ApiStock.Controller;
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
            await Task.Delay(2000);

            //while (!stoppingToken.IsCancellationRequested)
            //{
                var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

                _rabbitMqService.ReceiveMessages(async (message) =>
                {
                    var order = JsonSerializer.Deserialize<Order>(message)!;
                    _controller = new StockController(_rabbitMqService, dbContext);
                    _controller.CheckAvailability(order);
                });
            //    await Task.Delay(1000, stoppingToken);
            //}
            await Task.CompletedTask;
        }
    }

}
