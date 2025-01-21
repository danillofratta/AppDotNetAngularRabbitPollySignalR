using MediatR;

namespace ApiStock.Domain.Notifications
{
    public class ProductUpdateNotification : INotification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsDone { get; set; }
    }
}
