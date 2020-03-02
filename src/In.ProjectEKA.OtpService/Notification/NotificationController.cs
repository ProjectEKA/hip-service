namespace In.ProjectEKA.OtpService.Notification
{
    using System.Threading.Tasks;
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
        public async Task<ActionResult> SendNotification([FromBody] NotificationMessage notificationMessage)
        {
            return ReturnServerResponse( await notificationService.SendNotification(notificationMessage));
        }
        
        private ActionResult ReturnServerResponse(NotificationResponse notificationResponse)
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