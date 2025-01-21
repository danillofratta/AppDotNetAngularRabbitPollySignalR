using MediatR;

namespace ApiStock.Domain.Notifications
{
    public class ProductDeleteNotification : INotification
    {
        public int Id { get; set; }

        public bool IsDone { get; set; }
    }
}
