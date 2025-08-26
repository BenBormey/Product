using WebApplication1.Repository;
using WebApplication1.service;

namespace WebApplication1.Exspention
{
    public static class Handler
    {
        public static IServiceCollection serviceDescriptors(this IServiceCollection service)
        {
            service.AddScoped<MessageService>();
            service.AddScoped<CheckoutService>();
            service.AddScoped<IDashboardService, DashboardService>();
            service.AddSingleton<IQrService, QrService>();
            service.AddScoped<IPaymentService, DemoPaymentService>();

            return service;
        }
        

    }
}
