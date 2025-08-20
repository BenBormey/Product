using Microsoft.Extensions.Options;

namespace WebApplication1.Localization
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
        {
            var opts = app.ApplicationServices
                          .GetRequiredService<IOptions<RequestLocalizationOptions>>()
                          .Value;

            app.UseRequestLocalization(opts);
            return app;
        }
    }

}

