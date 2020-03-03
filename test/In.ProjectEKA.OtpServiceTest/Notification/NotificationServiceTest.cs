namespace In.ProjectEKA.OtpServiceTest.Notification
{
    using Builder;
    using FluentAssertions;
    using Moq;
    using OtpService.Notification;
    using Xunit;

    public class NotificationServiceTest
    {
        private readonly Mock<INotificationWebHandler> notificationWebHandler = new Mock<INotificationWebHandler>();
        private readonly NotificationService notificationService;

        public NotificationServiceTest()
        {
            notificationService = new NotificationService(notificationWebHandler.Object);
        }

        [Fact]
        private async void ReturnSuccessResponse()
        {
            var expectedResponse = new NotificationResponse(ResponseType.Success, "Notification sent");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                    It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessage());

            response.Should().Be(expectedResponse);
        }

        [Fact]
        private async void ReturnInternalServerError()
        {
            var expectedResponse = new NotificationResponse(ResponseType.InternalServerError, "Internal server error");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessage());

            response.ResponseType.Should().Be(expectedResponse.ResponseType);
        }

    }
}