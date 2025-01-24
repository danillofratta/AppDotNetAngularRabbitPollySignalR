using ApiOrder.Service.SignalIr;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using SharedDatabase.Models;

namespace ApiOrder.Service.ServiceCrud
{
    public class OrderServiceCrud
    {
        private readonly DBDevContext _context;

        public OrderServiceCrud(DBDevContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrdem(Order record)
        {
            record.Idstatus = 1;
            _context.Order.Add(record);
            await _context.SaveChangesAsync();

            return record;
        }

        public async Task<Order> UpdateStatusPaymentOk(Order order)
        {
            order.Idstatus = 8;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateStatusOutOfStock(Order order)
        {
            order.Idstatus = 6;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateStatusStockOk(Order order)
        {
            order.Idstatus = 7;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateStatusWaitPayment(Order order)
        {
            order.Idstatus = 2;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
