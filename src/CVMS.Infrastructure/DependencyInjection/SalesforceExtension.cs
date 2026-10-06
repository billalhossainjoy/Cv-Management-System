

using CVMS.Application.Integrations;
using CVMS.Infrastructure.Integrations;
using CVMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVMS.Infrastructure.DependencyInjection;

public static class SalesforceDependency
{
    public static IServiceCollection AddSalesforceExtension(
        this IServiceCollection services,
        IConfiguration configuration)
    {
    services.Configure<SalesforceOptions>(
        configuration.GetSection("Salesforce"));

    services.AddHttpClient<ISalesforceService, SalesforceService>();
    return services;
    }
}