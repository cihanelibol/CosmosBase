using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CosmosBase
{
    public class RabbitMqService : IMessageQueue
    {
        private readonly string HOSTNAME;

        public RabbitMqService(IConfiguration configuration)
        {
            HOSTNAME = configuration["RabbitMq:Host"];
        }
        public async Task SendAsync(string? queueName, string exchangeName, string? routingName, object data)
        {
            var factory = new ConnectionFactory { HostName = HOSTNAME };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
            await channel.QueueBindAsync(queueName, exchangeName, routingName);

            string jsonData = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(jsonData);

            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingName, body: body);
        }



        public async Task<object> ConsumeAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = HOSTNAME };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true);
            await channel.QueueBindAsync(queueName, exchangeName, routingKey);

            var consumer = new AsyncEventingBasicConsumer(channel);
            var tcs = new TaskCompletionSource<string>();

            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                tcs.SetResult(message);

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                return await tcs.Task;
            }
        }


    }
}
