namespace In.ProjectEKA.HipService.DataFlow
{
    public static class MessagingQueueConstants
    {
        public static readonly string DataRequestExchangeName = "hiservice.exchange.dataflowrequest.dotnetcore";
        public static readonly string DataRequestRoutingKey = "*.queue.durable.dotnetcore.#";
        public static readonly string DataRequestExchangeType = "topic";
    }
}