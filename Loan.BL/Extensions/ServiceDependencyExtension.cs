﻿using Common.Interfaces;
using Loan.BL.Services;
using Loan.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loan.BL.Extensions;

public static class ServiceDependencyExtension
{
    public static IServiceCollection AddLoanServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoanDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("LoanDatabase")));
        services.AddScoped<ILoanEmployeeService, LoanEmployeeService>();
        services.AddScoped<ILoanClientService, LoanClientService>();

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

