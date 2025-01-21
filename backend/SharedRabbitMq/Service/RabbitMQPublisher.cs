using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SharedRabbitMq.Service
{
    public interface IRabbitMQPublisher<T>
    {
        Task PublishMessageAsync(T message, string queueName);
    }

    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
    {
        public IConnection Connection { get; private set; }
        public IChannel Channel { get; set; }

        public async Task PublishMessageAsync(T message, string queueName)
        {

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            // Use o método assíncrono corretamente com 'await'
            Connection = await factory.CreateConnectionAsync();
            Channel = await Connection.CreateChannelAsync();

            Channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;

            await Channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, false, basicProperties: props, body: body);

            //await Task.Run(async () => Channel.BasicPublishAsync(exchange: "", routingKey: queueName, basicProperties: null, body: body));
        }
    }
}
