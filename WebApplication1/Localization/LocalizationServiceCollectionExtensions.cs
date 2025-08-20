using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace WebApplication1.Localization
{
    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddAppLocalization( this IServiceCollection services,IConfiguration config )
        {
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

            // read supported cultures from appsettings (optional)
            var cultureCodes = config.GetSection("localization:supportedCultures")
                                     .Get<string[]>() ?? new[] { "en-US", "km-KH" };

            var supportedCultures = cultureCodes.Select(c => new CultureInfo(c)).ToArray();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var defaultCulture = config["localization:defaultCulture"] ?? "en-US";

                opts.DefaultRequestCulture = new RequestCulture(defaultCulture);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;

                // priority: ?culture=km-KH → cookie → Accept-Language
                opts.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                opts.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
            });

            return services;
        }
    }
}
