
namespace In.ProjectEKA.HipService.MessagingQueue
{
    using Microsoft.Extensions.ObjectPool;
    using Microsoft.Extensions.Options;
    using RabbitMQ.Client;
    public class MessagingQueueModelPooledObjectPolicy : IPooledObjectPolicy<IModel> 
    {
        private readonly MessagingQueueOptions options;
        private readonly IConnection connection;  

        public MessagingQueueModelPooledObjectPolicy(IOptions<MessagingQueueOptions> optionsAccess)  
        {  
            options = optionsAccess.Value;  
            connection = GetConnection();  
        }
        
        private IConnection GetConnection()  
        {  
            var factory = new ConnectionFactory  
            {  
                HostName = options.HostName,
                Port = options.Port,  
            };  
  
            return factory.CreateConnection();  
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
    }
}