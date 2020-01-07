using Microsoft.AspNetCore.Builder;

namespace hip_service.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseConsentManagerIdentifierMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
        
    }
}