﻿using ApiOrder.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SharedDatabase.Dto;
using SharedDatabase.Models;
using SharedRabbitMq.Service;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;

namespace ApiSale.Controller
{
    //process    
    //1 action => process (separando estoque)
    //2 action => delivery => send order
    //3 action => deliverad => send order
    //4 action => done => send order
    //5 action => cancel delete sale and send order    

    [ApiController]
    [Route("api/v1/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly DBDevContext _context;
        public readonly RabbitMqService _rabbitMqService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public SaleController(RabbitMqService rabbitMqService, DBDevContext context, IHubContext<NotificationHub> hubContext)
        {
            _rabbitMqService = rabbitMqService;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<SaleDto>>> GetAll()
        {
            return Ok(await this.GetAllSaleDto());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<List<SaleDto>> GetAllSaleDto()
        {
            //todo nameproduct namestatus 
            //todo adddto
            //todo add foreign key e add include
            //List<SharedDatabase.Dto.OrderDto> list = _context.Order.ToList();
            var list = (from a in _context.Sale
                        join b in _context.Status on a.Idstatus equals b.Id
                        join c in _context.Product on a.Idproduct equals c.Id
                        select new SharedDatabase.Dto.SaleDto
                        {
                            Id = a.Id,
                            Idproduct = a.Amount,
                            Idcustomer = a.Idcustomer,
                            Idstatus = a.Idstatus,
                            Idorder = a.Idorder,
                            Price = a.Price,
                            Amount = a.Amount,
                            CreateAt = a.CreateAt,
                            namestatus = b.Name,
                            nameproduct = c.Name
                        }).ToList<SaleDto>();


            return list;
        }


        //todo remove item from stock
        [HttpPost("PaymentOK")]
        public async Task<ActionResult<Sale>> PaymentSale([FromBody] int idsale)
        {
            //if (record == null)
            //    return BadRequest("Sale is required");

            try
            {
                Sale sale =  await this.SaleStatusPayment(idsale, _context);

                Order order = await _context.Order.SingleAsync(x => x.Id == sale.Idorder);
                if (order != null)
                    await this.NotifyOrderPaymentOk(order);
            }
            catch (Exception)
            {
                return NotFound("Sale not found");
                throw;
            }
           
            
            return Ok("ok");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<Sale> SaleStatusPayment(int idsale, DBDevContext context)
        {
            //ApiExplorerSettings _context is null
            //Sale sale = await _context.Sale.SingleAsync(x => x.Id == idsale);
            Sale sale = await context.Sale.SingleAsync(x => x.Id == idsale);
            if (sale != null){
                sale.Idstatus = 8;
                _context.Sale.Update(sale);
                await _context.SaveChangesAsync();
                return sale;
            }

            return null;
        }


        private async Task NotifyOrderPaymentOk(Order order)
        {
            await _rabbitMqService.InitializeService();
            _rabbitMqService._queueName = "SaleToOrder-PaymentOK-Queue";
            _rabbitMqService.SendMessage(order);            
        }


        //não usar foi apenas para teste
        //[HttpPost]
        //public async Task<ActionResult<Sale>> CreateSale([FromBody] Order record)
        //{
        //    if (record == null)
        //        return BadRequest("Order is required");

        //    //todo method consume and return object order
        //    await this.ConsumeCreate();
        //    Sale sale = await this.CreateVendaDB(record);
        //    await this.CreateVendaSendOrdem(sale);

        //    return CreatedAtAction("GetSave", new { id = sale.Id }, sale);
        //}

        //private async Task ConsumeCreate()
        //{

        //}
        //private async Task<Sale> CreateVendaDB(Order record)
        //{
        //    var order = _context.Order.Single(o => o.Id == record.Id);
        //    Sale sale = new Sale();
        //    sale.Idorder = order.Id;
        //    sale.Idcustomer = order.Idcustomer;
        //    sale.Idproduct = order.Idproduct;
        //    sale.Value = order.Value;
        //    sale.CreateAt = DateTime.Now;
        //    sale.Idstatus = 2;
                        
        //    _context.Sale.Add(sale);
        //    await _context.SaveChangesAsync();

        //    return sale;
        //}

        //private async Task CreateVendaSendOrdem(Sale record)
        //{
        //    await _rabbitMqService.InitializeService();
        //    _rabbitMqService._queueName = "sale create";
        //    _rabbitMqService.SendMessage($"Registered Sale: {record.Id}, Value: {record.Value}");
        //}
    }
}
