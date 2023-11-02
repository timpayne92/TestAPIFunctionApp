using Azure.Data.Tables;
using Azure.Identity;
using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Test_API_Func.Common;
using Test_API_Func.Common.Models.Validator;
using Test_API_Func.Services;
using Test_API_Func.Services.Interfaces;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
        .AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddOptions<Settings>().Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("TableStorage").Bind(settings);
        });
        services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                    Title = DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle(),
                    Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription(),
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = DefaultOpenApiConfigurationOptions.GetOpenApiVersion(),
                IncludeRequestingHostName = DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
                ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
                ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced(),
            };

            return options;
        });
        services.AddSingleton(serviceProvider =>
        {
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            return new TableServiceClient(new Uri(Environment.GetEnvironmentVariable("AzureWebJobsStorage__tableServiceUri")), new DefaultAzureCredential());
        });
        services.AddTransient<ITableStorageService, TableStorageService>();
    })
    .Build();

host.Run();
