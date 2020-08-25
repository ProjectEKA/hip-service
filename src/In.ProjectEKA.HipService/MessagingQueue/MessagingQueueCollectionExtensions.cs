namespace In.ProjectEKA.HipService.MessagingQueue
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.ObjectPool;
    using RabbitMQ.Client;

    public static class MessagingQueueCollectionExtensions
    {
        public static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitConfig = configuration.GetSection("rabbit");
            services.Configure<MessagingQueueOptions>(rabbitConfig);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, MessagingQueueModelPooledObjectPolicy>();
            services.AddSingleton<IMessagingQueueManager, MessagingQueueManager>();
            return services;
        }
    }
}