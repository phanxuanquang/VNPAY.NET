
using Microsoft.OpenApi;
using VNPAY.Extensions;

namespace Backend_API_Testing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add VNPAY Payment Service
            var vnpayConfig = builder.Configuration.GetSection("VNPAY");

            builder.Services.AddVnpayClient(config =>
            {
                config.TmnCode = vnpayConfig["TmnCode"]!;
                config.HashSecret = vnpayConfig["HashSecret"]!;
                config.CallbackUrl = vnpayConfig["CallbackUrl"]!;
                // config.BaseUrl = vnpayConfig["BaseUrl"]!; // Tùy chọn. Nếu không thiết lập, giá trị mặc định là URL thanh toán môi trường TEST
                // config.Version = vnpayConfig["Version"]!; // Tùy chọn. Nếu không thiết lập, giá trị mặc định là "2.1.0"
                // config.OrderType = vnpayConfig["OrderType"]!; // Tùy chọn. Nếu không thiết lập, giá trị mặc định là "other"
            });
            #endregion

            builder.Services.AddControllers();

            #region Add Swagger UI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "VNPAY API with ASP.NET Core",
                    Version = "v1",
                    Description = "Created by Phan Xuan Quang",
                });
            });

            var app = builder.Build();

            app.UseSwagger(c =>
            {
                c.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
            #endregion

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}