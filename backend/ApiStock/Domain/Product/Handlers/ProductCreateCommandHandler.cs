using ApiStock.Domain.Product.Command;
using ApiStock.Domain.Product.Notifications;
using ApiStock.Domain.Product.Repository;
using MediatR;
using SharedDatabase.Models;

namespace ApiStock.Domain.Product.Handlers
{
    public class ProductCreateCommandHandler : IRequestHandler<ProductCreateCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;

        public ProductCreateCommandHandler(IMediator mediator, ProductRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<string> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {
            var product = new SharedDatabase.Models.Product { Name = request.Name, Price = request.Price };

            try
            {
                await _repository.SaveAsync(product);
                await _mediator.Publish(new ProductCreateNotification
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                });
                return await Task.FromResult("Produto criada com sucesso");
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new ProductCreateNotification { Id = product.Id, Name = product.Name, Price = product.Price });
                await _mediator.Publish(new ErroNotification { Erro = ex.Message, PilhaErro = ex.StackTrace });
                return await Task.FromResult("Ocorreu um erro no momento da criação");
            }
        }
    }
}
