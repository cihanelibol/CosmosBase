namespace CosmosBase
{
    interface IMessageQueue
    {
        public Task SendAsync(string queueName, string exchageName, string routingName, object data);
        public Task<object> ConsumeAsync(string queueName, string exchangeName, string routingKey, CancellationToken cancellationToken);
    }
}
