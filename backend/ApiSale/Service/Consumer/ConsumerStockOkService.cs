using ApiOrder.Service;
using ApiSale.Controller;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ApiSale.Service
{
    public class ConsumerStockOkService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;


        public ConsumerStockOkService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            //_rabbitMqService.InitializeService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "StockToSale-StockOK-Queue";
            await _rabbitMqService.InitializeService();
            await Task.Delay(2000);
           
            //while (!stoppingToken.IsCancellationRequested)
            //{
                var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

                _rabbitMqService.ReceiveMessages(async (message) =>
                {
                    var order = JsonSerializer.Deserialize<Order>(message)!;

                    this.CreateSale(order, dbContext);
                    this.NotifyOrder(order);
                });
            //    await Task.Delay(1000, stoppingToken);
            //}

            await Task.CompletedTask;
        }

        private async Task CreateSale(Order order, DBDevContext dbContext)
        {
            Sale sale = new Sale();
            sale.Idcustomer = order.Idcustomer;
            sale.Idproduct = order.Idproduct;
            sale.Idorder = order.Id;
            sale.Idstatus = 2;
            sale.Price = order.Price;
            sale.Amount = order.Amount;
            sale.CreateAt = DateTime.Now;
            
            await dbContext.Sale.AddAsync(sale);
            await dbContext.SaveChangesAsync();

            SaleController oSaleController = new SaleController(_rabbitMqService, dbContext, _hubContext);
            List<SaleDto> list = await oSaleController.GetAllSaleDto();

            await _hubContext.Clients.All.SendAsync("GetListSale", list);
        }

        //todo put in core domain service
        private async Task NotifyOrder(Order order)
        {
            await _rabbitMqService.InitializeService();
            _rabbitMqService._queueName = "SaleToOrder-WaitPayment-Queue";
            _rabbitMqService.SendMessage(order);
            //_rabbitMqService.SendMessage("Stock to Order => Product quantity in stock ok");
        }

    }

}
