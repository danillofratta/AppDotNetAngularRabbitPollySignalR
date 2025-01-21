using ApiStock.Domain.Command;
using ApiStock.Domain.Notifications;
using ApiStock.Domain.Repository;
using MediatR;
using SharedDatabase.Models;

namespace ApiStock.Domain.Handlers
{
    public class ProductDeleteCommandHandler : IRequestHandler<ProductDeleteCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;

        public ProductDeleteCommandHandler(IMediator mediator, ProductRepository repository)
        {
            this._mediator = mediator;
            this._repository = repository;
        }
        public async Task<string> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var product = new Product { Id = request.Id };

            try
            {
                await _repository.DeleteAsync(product);
                await _mediator.Publish(new ProductDeleteNotification
                {
                    Id = product.Id,
                    IsDone = true
                });
                return await Task.FromResult("Produto excluido com sucesso");
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new ProductDeleteNotification { Id = product.Id, IsDone = false});
                await _mediator.Publish(new ErroNotification { Erro = ex.Message, PilhaErro = ex.StackTrace });
                return await Task.FromResult("Ocorreu um erro no momento da criação");
            }
        }
    }
}
