using Microsoft.Extensions.DependencyInjection;
using System;
using VNPAY.Extensions.Options;

namespace VNPAY.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVnpayPayment(this IServiceCollection services, Action<VnpayConfiguration> config)
        {
            services.Configure(config);
            services.AddHttpContextAccessor();
            services.AddScoped<IVnpayClient, VnpayClient>();
            return services;
        }
    }
}