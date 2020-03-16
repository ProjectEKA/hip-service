namespace In.ProjectEKA.HipService
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
            Func<IApplicationBuilder, IApplicationBuilder> action)
        {
            return predicate ? action(application) : application;
        }

        public static IApplicationBuilder UseCustomOpenApi(this IApplicationBuilder application)
        {
            return application.UseSwaggerUI(options => { options.SwaggerEndpoint("/hip-spec.yaml", "hip"); })
                .UseReDoc(options => { options.SpecUrl("/hip-spec.yaml"); });
        }
    }
}