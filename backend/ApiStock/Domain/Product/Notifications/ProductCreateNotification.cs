using MediatR;

namespace ApiStock.Domain.Product.Notifications
{
    public class ProductCreateNotification : INotification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
