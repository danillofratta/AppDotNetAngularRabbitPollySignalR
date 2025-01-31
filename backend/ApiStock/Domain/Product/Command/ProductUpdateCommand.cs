using MediatR;

namespace ApiStock.Domain.Product.Command
{
    public class ProductUpdateCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
