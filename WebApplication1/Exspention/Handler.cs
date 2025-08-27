using FluentValidation;
using WebApplication1.Entities;
using WebApplication1.Models.Product;
using WebApplication1.Repository;
using WebApplication1.service;
using WebApplication1.Validator;

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
            service.AddScoped<IpromotionRepository, PromotionService>();
            service.AddScoped<IValidator<CreateProductVM>, CreateProductValidator>();
            service.AddValidatorsFromAssemblyContaining<ProductEditVMValidator>();

            return service;
        }
        

    }
}
