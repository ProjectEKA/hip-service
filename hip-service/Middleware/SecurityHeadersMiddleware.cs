using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace hip_service.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            IHeaderDictionary headers = context.Request.Headers;
            headers["X-ConsentManagerID"] = "ncgConsentManager";
            await _next(context);
        }
    }
}