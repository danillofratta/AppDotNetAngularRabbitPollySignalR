using ApiOrder.Enun;
using ApiOrder.Service.Query;
using ApiOrder.Service.SignalR;
using ApiSale.Controller;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;

namespace ApiOrder.Service.ServiceCrud
{
    public class OrderServiceCrud
    {
        private readonly DBDevContext _context;
        private readonly ILogger<OrderController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly OrderServiceQuery _query;
        public OrderServiceCrud(ILogger<OrderController> logger, DBDevContext context, IHubContext<NotificationHub> hubContext, OrderServiceQuery query)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
            _query = query;
        }

        public async Task<Order> CreateOrder(Order record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            try
            {
                record.Idstatus = (int)OrderStatus.Pending;
                _context.Order.Add(record);
                await _context.SaveChangesAsync();

                return record;
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Error creating order");
                throw new InvalidOperationException("Error creating order", ex);
            }            
        }

        public async Task<Order> UpdateOrderStatusAsync(Order order, OrderStatus status)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                Order record = await _context.Order.SingleOrDefaultAsync(x => x.Id == order.Id);

                //order.Idstatus = (int)status;
                record.Idstatus = (int)status;
                _context.Entry(record).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                List<OrderDto> list = await this._query.GetAllOrderDto();
                await _hubContext.Clients.All.SendAsync("GetListOrder", list);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status to {status}");
                throw new InvalidOperationException($"Error updating order status to {status}", ex);
            }
        }
    }
}
