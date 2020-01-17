namespace In.ProjectEKA.HipService.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            headers["X-ConsentManagerID"] = "ncgConsentManager";
            await _next(context);
        }
    }
}