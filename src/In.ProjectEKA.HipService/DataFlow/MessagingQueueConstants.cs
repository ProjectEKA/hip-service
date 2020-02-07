namespace In.ProjectEKA.HipService.DataFlow
{
    public static class MessagingQueueConstants
    {
        public static readonly string DataRequestExchangeName = "hiservice.exchange.dataflowrequest";
        public static readonly string DataRequestRoutingKey = "*.queue.durable.dataflowrequest.#";
        public static readonly string DataRequestExchangeType = "topic";
    }
}