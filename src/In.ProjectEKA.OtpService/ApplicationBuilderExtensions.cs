namespace In.ProjectEKA.OtpService
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.FileProviders;
    
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseStaticFilesWithYaml(this IApplicationBuilder application)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".yaml"] = "application/x-yaml";
            return application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                ContentTypeProvider = provider
            });
        }
        
        public static IApplicationBuilder UseIf(
            this IApplicationBuilder application,
            bool predicate,
            Func<IApplicationBuilder, IApplicationBuilder> action) =>
            predicate ? action(application) : application;

        public static IApplicationBuilder UseCustomOpenAPI(this IApplicationBuilder application) =>
            application.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger.yaml", "OtpServer");
            }).UseReDoc(options => {
                options.SpecUrl("/swagger.yaml");
            });
    }
}