namespace In.ProjectEKA.HipService.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseConsentManagerIdentifierMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}