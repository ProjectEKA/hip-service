namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.ObjectPool;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class MessagingQueueListener : BackgroundService
    {
        private readonly DataFlowMessageHandler dataFlowMessageHandler;
        private readonly DefaultObjectPool<IModel> objectPool;
        private IModel channel;

        public MessagingQueueListener(IPooledObjectPolicy<IModel> objectPolicy,
            DataFlowMessageHandler dataFlowMessageHandler)
        {
            this.dataFlowMessageHandler = dataFlowMessageHandler;
            objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
            SetupMessagingQueue();
        }

        private void SetupMessagingQueue()
        {
            channel = objectPool.Get();
            channel.ExchangeDeclare(MessagingQueueConstants.DataRequestExchangeName, ExchangeType.Topic, true);
            channel.QueueDeclare(
                MessagingQueueConstants.DataRequestRoutingKey,
                true,
                true,
                false,
                null);
            channel.QueueBind(
                MessagingQueueConstants.DataRequestRoutingKey,
                MessagingQueueConstants.DataRequestExchangeName,
                MessagingQueueConstants.DataRequestRoutingKey,
                null);
            channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var dataFlowMessage =
                    JsonConvert.DeserializeObject<HipLibrary.Patient.Model.TraceableDataRequest>(message);
                await dataFlowMessageHandler.HandleDataFlowMessage(dataFlowMessage);
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(MessagingQueueConstants.DataRequestRoutingKey, false, consumer);
            return Task.CompletedTask;
        }
    }
}