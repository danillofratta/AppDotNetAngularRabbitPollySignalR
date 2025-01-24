using ApiOrder.Service.SignalIr;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;

namespace ApiOrder.Service.Query
{
    public class OrderServiceQuery
    {
        private readonly DBDevContext _context;
        
        public OrderServiceQuery(DBDevContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> GetAllOrderDto()
        {
            var list = await (from a in _context.Order
                              join b in _context.Status on a.Idstatus equals b.Id
                              join c in _context.Product on a.Idproduct equals c.Id
                              select new SharedDatabase.Dto.OrderDto
                              {
                                  Id = a.Id,
                                  Idproduct = a.Amount,
                                  Idcustomer = a.Idcustomer,
                                  Idstatus = a.Idstatus,
                                  Price = a.Price,
                                  Amount = a.Amount,
                                  CreateAt = a.CreateAt,
                                  namestatus = b.Name,
                                  nameproduct = c.Name
                              }).ToListAsync<OrderDto>();

            return list;
        }
    }
}
