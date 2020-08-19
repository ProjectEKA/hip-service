namespace In.ProjectEKA.HipService.MessagingQueue
{
    public interface IMessagingQueueManager
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;
    }
}