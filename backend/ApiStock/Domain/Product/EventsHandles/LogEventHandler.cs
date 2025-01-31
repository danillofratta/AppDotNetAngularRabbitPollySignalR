using ApiStock.Domain.Product.Notifications;
using MediatR;

namespace ApiStock.Domain.Product.EventsHandles
{
    public class LogEventHandler :
                            INotificationHandler<ProductCreateNotification>,
                            INotificationHandler<ProductUpdateNotification>,
                            INotificationHandler<ProductDeleteNotification>,
                            INotificationHandler<ErroNotification>
    {
        public Task Handle(ProductCreateNotification notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"CRIACAO: '{notification.Id} " +
                    $"- {notification.Name} - {notification.Price}'");
            });
        }

        public Task Handle(ProductUpdateNotification notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ALTERACAO: '{notification.Id} - {notification.Name} " +
                    $"- {notification.Price} - {notification.IsDone}'");
            });
        }

        public Task Handle(ProductDeleteNotification notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"EXCLUSAO: '{notification.Id} " +
                    $"- {notification.IsDone}'");
            });
        }

        public Task Handle(ErroNotification notification, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ERRO: '{notification.Erro} \n {notification.PilhaErro}'");
            });
        }
    }
}
