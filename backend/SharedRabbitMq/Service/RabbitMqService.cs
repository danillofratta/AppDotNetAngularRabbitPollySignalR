using Microsoft.Extensions.Hosting;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedRabbitMq.Service
{
    public class RabbitMqService : IDisposable
    {
        protected const string VirtualHost = "CUSTOM_HOST";
        protected readonly string LoggerExchange = $"{VirtualHost}.LoggerExchange";

        //_hostname = "localhost" dev
        //_hostname = "rabbitmq" docker
        public string _hostname = "";  // Nome do host do RabbitMQ
        //public string _hostname = "localhost";  // Nome do host do RabbitMQ
        //todo pass nome da fila
        public string _queueName = ""; // Nome da fila

        public string _user = "guest";
        public string _password = "guest";

        public IConnection Connection { get; private set; }
        public IChannel Channel { get; set; }

        public async Task InitializeService()
        {
            #if DEBUG
                _hostname = "localhost";
            #else
                _hostname = "rabbitmq";
            #endif

            // Configura o Polly para retry e circuit breaker para RabbitMQ
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));  // Retry exponencial

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreaker(3, TimeSpan.FromSeconds(10)); // Abre o circuito após 3 falhas consecutivas

            await retryPolicy.Execute(async () =>
            {
                await circuitBreakerPolicy.Execute(async () =>
                {
                    var factory = new ConnectionFactory() { HostName = _hostname, Port = 5672, UserName = this._user, Password = this._password };

                    // Use o método assíncrono corretamente com 'await'
                    Connection = await factory.CreateConnectionAsync();
                    Channel = await Connection.CreateChannelAsync();

                    //Channel.ExchangeDeclareAsync(
                    //    exchange: LoggerExchange,
                    //    type: "direct",
                    //    durable: true,
                    //    autoDelete: false);

                    await Channel.QueueDeclareAsync(
                        queue: _queueName, 
                        durable: false, 
                        exclusive: false, 
                        autoDelete: false, 
                        arguments: null);
                });
            });
        }

        public async Task SendMessage<T>(T message)
        {
            //var body = Encoding.UTF8.GetBytes(message);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;
            props.Persistent = true;

            await Channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, false, basicProperties: props, body: body);

            Console.WriteLine($"Mensagem enviada: {message}");
        }

        // Método para receber mensagens da fila RabbitMQ
        public async Task ReceiveMessages(Action<string> handleMessage)
        {
            // Usando EventingBasicConsumer
            var consumer = new AsyncEventingBasicConsumer(Channel);

            // Definindo o que fazer ao receber uma mensagem
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Mensagem recebida: {message}");
                handleMessage(message);

                await Channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            // Iniciando o consumo das mensagens
            await Channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            try
            {
                Channel?.CloseAsync();
                Channel?.DisposeAsync();
                Channel = null;

                Connection?.CloseAsync();
                Connection?.DisposeAsync();
            }
            catch (Exception ex)
            {
                
            }
        }
    }

    
}

