using Common.Interfaces;
using Core.BL.Services;
using Core.Common.Interfaces;
using Core.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.BL;
public static class ServiceDependencyExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CoreDatabase")));

        services.AddScoped<ICoreService, CoreService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<RabbitMQProducer>();
        services.AddHostedService<RabbitMQConsumer>();
        services.AddHttpClient<Converter>();

        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config["Redis:ConnectionString"];
            options.InstanceName = config["Redis:InstanceName"];
        });
        return services;
    }
}
