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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "StockToSale-StockOK-Queue";
            await _rabbitMqService.InitializeService();
           
            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBDevContext>();

            await _rabbitMqService.ReceiveMessages(async (message) =>
            {
                var order = JsonSerializer.Deserialize<Order>(message)!;

                await this.CreateSale(order, dbContext);
                await this.NotifyOrder(order);
            });            
        }

        //todo craete SaleServiceCrud
        private async Task CreateSale(Order order, DBDevContext dbContext)
        {            
            try
            {
                Sale sale = new Sale();
                sale.Idcustomer = order.Idcustomer;
                sale.Idproduct = order.Idproduct;
                sale.Idorder = order.Id;
                sale.Idstatus = 2;
                sale.Price = order.Price;
                sale.Amount = order.Amount;
                sale.CreateAt = DateTime.Now;

                dbContext.Sale.Add(sale);
                dbContext.SaveChanges();
                
                SaleController oSaleController = new SaleController(_rabbitMqService, dbContext, _hubContext);
                await oSaleController.GetAllSaleDto();
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        private async Task NotifyOrder(Order order)
        {
            _rabbitMqService._queueName = "SaleToOrder-WaitPayment-Queue";
            await _rabbitMqService.InitializeService();
            await _rabbitMqService.SendMessage(order);
        }
    }

}
