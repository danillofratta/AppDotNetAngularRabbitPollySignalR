using ApiSale.Service.SignalR;
using Microsoft.AspNetCore.SignalR;
using SharedDatabase.Dto;
using SharedDatabase.Models;

namespace ApiSale.Service.Query
{
    public class SaleQueryService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly DBDevContext _context;

        public SaleQueryService(DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<List<SaleDto>> GetAllSaleDto()
        {
            var list = (from a in _context.Sale
                        join b in _context.Status on a.Idstatus equals b.Id
                        join c in _context.Product on a.Idproduct equals c.Id
                        select new SharedDatabase.Dto.SaleDto
                        {
                            Id = a.Id,
                            Idproduct = a.Idproduct,
                            Idcustomer = a.Idcustomer,
                            Idstatus = a.Idstatus,
                            Idorder = a.Idorder,
                            Price = a.Price,
                            Amount = a.Amount,
                            CreateAt = a.CreateAt,
                            namestatus = b.Name,
                            nameproduct = c.Name
                        }).ToList<SaleDto>();


            await _hubContext.Clients.All.SendAsync("GetListSale", list);

            return list;
        }
    }
}
