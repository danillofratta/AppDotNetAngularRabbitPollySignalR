using ApiStock.Domain.Product.Command;
using ApiStock.Domain.Product.Notifications;
using ApiStock.Domain.Product.Repository;
using MediatR;
using SharedDatabase.Models;

namespace ApiStock.Domain.Product.Handlers
{
    public class ProductDeleteCommandHandler : IRequestHandler<ProductDeleteCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ProductRepository _repository;

        public ProductDeleteCommandHandler(IMediator mediator, ProductRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<string> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var product = new SharedDatabase.Models.Product { Id = request.Id };

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
                await _mediator.Publish(new ProductDeleteNotification { Id = product.Id, IsDone = false });
                await _mediator.Publish(new ErroNotification { Erro = ex.Message, PilhaErro = ex.StackTrace });
                return await Task.FromResult("Ocorreu um erro no momento da criação");
            }
        }
    }
}
