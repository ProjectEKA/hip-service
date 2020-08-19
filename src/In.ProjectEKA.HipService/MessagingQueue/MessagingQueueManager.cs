namespace In.ProjectEKA.HipService.MessagingQueue
{
    using System;
    using System.Text;
    using Logger;
    using Microsoft.Extensions.ObjectPool;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class MessagingQueueManager : IMessagingQueueManager
    {
        private readonly DefaultObjectPool<IModel> objectPool;

        public MessagingQueueManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class
        {
            if (message == null) return;
            var channel = objectPool.Get();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception);
            }
            finally
            {
                objectPool.Return(channel);
            }
        }
    }
}