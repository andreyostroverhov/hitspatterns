using Account.BL.Services;
using Account.DAL.Data;
using Common.Interfaces;
using Common.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Account.BL.Extensions;

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension
{
    /// <summary>
    /// Add BackendAPI BL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddUserBlServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AccountDatabase")));

        services.AddDbContext<MonitoringDbContext>(options =>
           options.UseNpgsql(configuration.GetConnectionString("MonitoringDatabase"),
        b => b.MigrationsAssembly("Common")));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();


        services.AddHostedService<MonitoringEventConsumer>();
        services.AddSingleton<IMonitoringPublisher>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<RabbitMQMonitoringPublisher>>();

            return new RabbitMQMonitoringPublisher(
                hostName: configuration["RabbitMQ:HostName"] ?? throw new ArgumentNullException("HostName"),
                userName: configuration["RabbitMQ:UserName"] ?? throw new ArgumentNullException("UserName"),
                password: configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("Password"),
                serviceName: "AccountService",
                logger: logger
            );
        });

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

