using Microsoft.Extensions.DependencyInjection;
using VNPAY.Extensions.Options;

namespace VNPAY.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVnPayPayment(this IServiceCollection services, Action<VnpayConfigurations> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddHttpContextAccessor();
            services.AddScoped<IVnpay, Vnpay>();
            return services;
        }
    }
}