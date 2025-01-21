
using MediatR;

namespace ApiStock.Domain.Command
{
    public class ProductDeleteCommand : IRequest<string>
    {
        public int Id { get; set; }
    }
}
