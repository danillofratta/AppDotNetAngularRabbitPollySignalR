using MediatR;

namespace ApiStock.Domain.Product.Command
{
    public class ProductDeleteCommand : IRequest<string>
    {
        public int Id { get; set; }
    }
}
