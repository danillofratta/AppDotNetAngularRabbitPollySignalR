using ApiStock.Domain.Command;
using ApiStock.Domain.Repository;
using Domain.Repository.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedDatabase.Models;

namespace ApiStock.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;
        public ProductController(IMediator mediator, ProductRepository repository)
        {
            this._mediator = mediator;
            this._repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _repository.GetById(id));
        }

        [HttpGet("getbyname/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            return Ok(await _repository.GetByName(name));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductCreateCommand command)
        { 
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ProductUpdateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = new ProductDeleteCommand { Id = id };
            var result = await _mediator.Send(obj);
            return Ok(result);
        }
    }
}
