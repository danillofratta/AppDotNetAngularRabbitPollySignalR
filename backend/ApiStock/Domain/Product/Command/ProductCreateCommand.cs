using MediatR;

namespace ApiStock.Domain.Product.Command
{
    public class ProductCreateCommand : IRequest<string>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
