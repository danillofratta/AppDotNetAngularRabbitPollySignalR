using MediatR;

namespace ApiStock.Domain.Notifications
{
    public class ProductCreateNotification : INotification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
