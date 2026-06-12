
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Core.Services;
using Heralabs.ClientManagement.Infrastructure.Data;
using Heralabs.ClientManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Heralabs.ClientManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddLog4Net("log4net.config");

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DB_Entities")));

            RegisterServices(builder);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();            

            app.MapControllers();

            var packagesPath = Path.Combine(builder.Environment.ContentRootPath, "mobile-packages");
            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
            }

            // Register MIME Type for APK (Android), IPA (IOS)
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".apk"] = "application/vnd.android.package-archive";
            provider.Mappings[".ipa"] = "application/octet-stream"; 

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(packagesPath),
                RequestPath = "/mobile-packages",
                ContentTypeProvider = provider
            });

            app.Run();
        }

        private static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IMobileAppVersionRepository, MobileAppVersionRepository>();
            builder.Services.AddScoped<IWebApiVersionRepository, WebApiVersionRepository>();
            builder.Services.AddScoped<IClientCacheService, ClientCacheService>();
            builder.Services.AddScoped<IAppVersionCacheService, AppVersionCacheService>();
        }
    }
}
