namespace In.ProjectEKA.HipService.MessagingQueue
{
    using Microsoft.Extensions.ObjectPool;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;

    public class MessagingQueueModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly IConnection connection;
        private readonly MessagingQueueOptions options;

        public MessagingQueueModelPooledObjectPolicy(IOptions<MessagingQueueOptions> optionsAccess)
        {
            options = optionsAccess.Value;
            connection = GetConnection();
        }

        public IModel Create()
        {
            return connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            obj.Dispose();
            return false;
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            return factory.CreateConnection();
        }
    }
}