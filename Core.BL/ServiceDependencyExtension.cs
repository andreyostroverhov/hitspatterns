using Common.Interfaces;
using Common.Monitoring;
using Common.Policies;
using Core.BL.Services;
using Core.Common.Interfaces;
using Core.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.Extensions.Http;
using Polly;

namespace Core.BL;
public static class ServiceDependencyExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CoreDatabase")));

        services.AddScoped<ICoreService, CoreService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddSingleton<FirebaseService>();

        services.AddSingleton<RabbitMQProducer>(provider =>
            new RabbitMQProducer(
                configuration["RabbitMQ:HostName"],
                configuration["RabbitMQ:UserName"],
                configuration["RabbitMQ:Password"],
                configuration["RabbitMQ:QueueName"]
            ));
        services.AddHostedService<RabbitMQConsumer>();

        services.AddHttpClient<Converter>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  // Sample: default lifetime is 2 minutes
            .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy());
        services.AddHttpClient<UserServise>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))  // Sample: default lifetime is 2 minutes
            .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy());


        services.AddHostedService<MonitoringEventConsumer>();
        services.AddSingleton<IMonitoringPublisher>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<RabbitMQMonitoringPublisher>>();
            return new RabbitMQMonitoringPublisher(
                hostName: configuration["RabbitMQ:HostName"]!,
                userName: configuration["RabbitMQ:UserName"]!,
                password: configuration["RabbitMQ:Password"]!,
                serviceName: "CoreService",
                logger: logger
            );
        });

        return services;
    }
}
