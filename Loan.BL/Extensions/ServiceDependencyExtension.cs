using Common.Interfaces;
using Common.Monitoring;
using Common.Policies;
using Loan.BL.Services;
using Loan.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Loan.BL.Extensions;

public static class ServiceDependencyExtension
{
    public static IServiceCollection AddLoanServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoanDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("LoanDatabase")));
        services.AddScoped<ILoanEmployeeService, LoanEmployeeService>();
        services.AddScoped<ILoanClientService, LoanClientService>();


        services.AddHostedService<MonitoringEventConsumer>();
        services.AddSingleton<IMonitoringPublisher, RabbitMQMonitoringPublisher>();
        services.AddSingleton<IMonitoringPublisher>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<RabbitMQMonitoringPublisher>>();

            return new RabbitMQMonitoringPublisher(
                hostName: configuration["RabbitMQ:HostName"] ?? throw new ArgumentNullException("HostName"),
                userName: configuration["RabbitMQ:UserName"] ?? throw new ArgumentNullException("UserName"),
                password: configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("Password"),
                serviceName: "LoanService", // Уникальное имя для каждого сервиса
                logger: logger
            );
        });

        services.AddHttpClient<CoreService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  // Sample: default lifetime is 2 minutes
            .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy()); ;


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

