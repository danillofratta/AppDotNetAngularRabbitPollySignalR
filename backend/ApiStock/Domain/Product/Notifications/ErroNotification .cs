using MediatR;

namespace ApiStock.Domain.Product.Notifications
{
    public class ErroNotification : INotification
    {
        public string Erro { get; set; }
        public string PilhaErro { get; set; }
    }
}
