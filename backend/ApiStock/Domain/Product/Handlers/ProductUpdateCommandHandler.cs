using ApiStock.Domain.Product.Command;
using ApiStock.Domain.Product.Notifications;
using ApiStock.Domain.Product.Repository;
using MediatR;
using SharedDatabase.Models;

namespace ApiStock.Domain.Product.Handlers
{
    public class ProductUpdateCommandHandler : IRequestHandler<ProductUpdateCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;

        public ProductUpdateCommandHandler(IMediator mediator, ProductRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<string> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var product = new SharedDatabase.Models.Product { Id = request.Id, Name = request.Name, Price = request.Price };

            try
            {
                await _repository.UpdateAsync(product);
                await _mediator.Publish(new ProductUpdateNotification
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    IsDone = true
                });
                return await Task.FromResult("Produto criada com sucesso");
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new ProductUpdateNotification { Id = product.Id, Name = product.Name, Price = product.Price, IsDone = false });
                await _mediator.Publish(new ErroNotification { Erro = ex.Message, PilhaErro = ex.StackTrace });
                return await Task.FromResult("Ocorreu um erro no momento da criação");
            }
        }
    }
}
