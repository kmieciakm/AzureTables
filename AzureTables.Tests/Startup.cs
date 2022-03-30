using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTables.Tests;

internal class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(settings =>
            new AzureStorageSettings()
            {
                ConnectionString = "UseDevelopmentStorage=true;"
            }
        );
        services.AddScoped<ITestTable, TestTable>();
    }

    public void Configure(IApplicationBuilder app)
    {
    }
}
