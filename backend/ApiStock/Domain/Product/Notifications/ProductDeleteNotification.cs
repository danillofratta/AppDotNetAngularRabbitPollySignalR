using MediatR;

namespace ApiStock.Domain.Product.Notifications
{
    public class ProductDeleteNotification : INotification
    {
        public int Id { get; set; }

        public bool IsDone { get; set; }
    }
}
