using VNPAY.NET.Extensions;

namespace Backend_API_Testing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var vnpayConfig = builder.Configuration.GetSection("Vnpay");

            builder.Services.AddVnPayPayment(options =>
            {
                options.TmnCode = vnpayConfig["TmnCode"]!;
                options.HashSecret = vnpayConfig["HashSecret"]!;
                options.BaseUrl = vnpayConfig["BaseUrl"]!;
                options.CallbackUrl = vnpayConfig["CallbackUrl"]!;
            });

            builder.Services.AddControllers();

            // Add Swagger UI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "VNPAY API with ASP.NET Core",
                    Version = "v1",
                    Description = "Created by Phan Xuan Quang"
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
