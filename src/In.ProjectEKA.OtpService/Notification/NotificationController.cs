namespace In.ProjectEKA.OtpService.Notification
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/notification")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        
        [HttpPost]
        public async Task<ActionResult> SendNotification([FromBody] Notification notification)
        {
            return ResponseFrom( await notificationService.SendNotification(notification));
        }
        
        private ActionResult ResponseFrom(Response notificationResponse)
        {
            return notificationResponse.ResponseType switch
            {
                ResponseType.Success => (ActionResult) Ok(notificationResponse),
                ResponseType.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, notificationResponse),
                _ => NotFound(notificationResponse)
            };
        }

    }
}