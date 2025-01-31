using ApiOrder.Enun;
using ApiOrder.Service.Query;
using ApiOrder.Service.ServiceCrud;
using ApiOrder.Service.SignalR;
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

namespace ApiOrder.Service.RabbitMq.Consumer
{
    public class ConsumerWaitPaymentService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly OrderServiceQuery _query;
        private readonly OrderServiceCrud _servicecrud;

        public ConsumerWaitPaymentService(OrderServiceCrud servicecrud, OrderServiceQuery query, RabbitMqService rabbitMqService, IHubContext<NotificationHub> hubContext)
        {
            _servicecrud = servicecrud;
            _query = query;
            _rabbitMqService = rabbitMqService;            
            _hubContext = hubContext;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService._queueName = "SaleToOrder-WaitPayment-Queue";
            await _rabbitMqService.InitializeService();

            await _rabbitMqService.ReceiveMessages(async (message) =>
            {
                var order = JsonSerializer.Deserialize<Order>(message)!;

                await OrderStatusWaitPayment(order);
            });
        }

        private async Task OrderStatusWaitPayment(Order order)
        {
            await _servicecrud.UpdateOrderStatusAsync(order, OrderStatus.WaitingPayment);
        }

        // Pass CancellationToken for graceful shutdown
        //await _rabbitMqService.ReceiveMessages(async (message, deliveryTag) =>
        //    {
        //    try
        //    {
        //        var order = JsonSerializer.Deserialize<Order>(message);

        //        if (order == null)
        //        {
        //            Console.WriteLine("Received a null or invalid order message.");
        //            return;
        //        }

        //        await OrderStatusWaitPayment(order);

        //        // Acknowledge the message after successful processing
        //        await _rabbitMqService.AcknowledgeMessage(deliveryTag);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error processing message: {ex.Message}");
        //    }
        //}, stoppingToken);
        //}

    }

}
