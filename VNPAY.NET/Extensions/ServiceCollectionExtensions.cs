using Microsoft.Extensions.DependencyInjection;
using VNPAY.NET.Extensions.Options;

namespace VNPAY.NET.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVnPayPayment(this IServiceCollection services, Action<VnpayOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddHttpContextAccessor();
            services.AddScoped<IVnpay, Vnpay>();
            return services;
        }
    }
}