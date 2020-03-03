namespace In.ProjectEKA.OtpServiceTest.Notification.Builder
{
    using Bogus;
    using Newtonsoft.Json.Linq;
    using OtpService.Notification;

    public static class TestBuilder
    {
        private static Faker faker;
        internal static Faker Faker() => faker ??= new Faker();

        public static Notification GenerateNotificationMessage()
        {
            var fakerNotification = Faker();
            return new Notification(fakerNotification.Random.Hash(),
                                            new Communication(CommunicationType.Mobile, fakerNotification.Random.Words(10)), 
                                            new JObject(), 
                                            Action.ConsentRequestCreated);
        }
    }
}