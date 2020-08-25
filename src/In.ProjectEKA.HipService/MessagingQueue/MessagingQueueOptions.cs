namespace In.ProjectEKA.HipService.MessagingQueue
{
    public class MessagingQueueOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}