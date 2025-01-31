using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Polly;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SharedDatabase.Dto;
using Microsoft.AspNetCore.SignalR;
using ApiStock.Service.SignalR;

namespace ApiStock.Controller
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public StockController(RabbitMqService rabbitMqService, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;         
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost()]
        public async Task<ActionResult> AddProductIntoStock([FromBody] StockDto dto)
        {
            try
            {
                //todo add bussines class
                Stock stock = await _context.Stock.FirstOrDefaultAsync(x => x.Idproduct == dto.idproduct);
                if (stock != null)
                {
                    var total = await _context.Stock.Where(x => x.Idproduct == dto.idproduct).SumAsync(x => x.Amount);
                    stock.Amount = total + dto.amount;

                    _context.Entry(stock).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await _context.SaveChangesAsync();

                    //return Ok($"Product with total {stock.Amount} in Stock");
                    this.GetAll();
                    return Ok(stock);
                }
                else
                {
                    stock = new Stock();
                    stock.Idproduct = dto.idproduct;
                    stock.Amount = dto.amount;
                    _context.Entry(stock).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    await _context.SaveChangesAsync();

                    //return Ok($"Product with total {stock.Amount} in Stock");
                    this.GetAll();
                    return Ok(stock);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Product and Amount is required => " + ex);
                throw ex;
            }                                        
        }

        [HttpGet]
        public async Task<ActionResult<List<StockDto>>> GetAll()
        {
            var list = (from a in _context.Stock                        
                        join b in _context.Product on a.Idproduct equals b.Id
                        select new SharedDatabase.Dto.StockDto
                        {
                            id = a.Id,
                            idproduct = a.Idproduct,                            
                            amount = a.Amount,
                            nameproduct = b.Name
                        }).ToList<StockDto>();

            await _hubContext.Clients.All.SendAsync("GetListStock", list);

            return Ok(list);
        }      
    }
}

