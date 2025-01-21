using ApiStock.Domain.Command;
using ApiStock.Domain.Notifications;
using ApiStock.Domain.Repository;
using MediatR;
using SharedDatabase.Models;

namespace ApiStock.Domain.Handlers
{
    public class ProductUpdateCommandHandler : IRequestHandler<ProductUpdateCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;

        public ProductUpdateCommandHandler(IMediator mediator, ProductRepository repository)
        {
            this._mediator = mediator;
            this._repository = repository;
        }
        public async Task<string> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var product = new Product { Id = request.Id, Name = request.Name, Price = request.Price };

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
