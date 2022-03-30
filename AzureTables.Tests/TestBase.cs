using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTables.Tests;


public abstract class TestBase : IDisposable
{
    protected TestServer TestServer { get; private set; }
    protected HttpClient TestClient { get; private set; }

    protected ITestcontainersBuilder<TestcontainersContainer> AzuriteBuilder { get; }

    public TestBase()
    {
        TestServer = new(
            new WebHostBuilder()
                .UseStartup<Startup>()
        );
        TestClient = TestServer.CreateClient();

        AzuriteBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/azure-storage/azurite")
          .WithName("azurite")
          .WithPortBinding(10002, 10002);
    }

    public AzureStorageSettings? GetStorageSettings()
    {
        return TestServer.Services
            .GetService<AzureStorageSettings>();
    }

    public ITestTable? GetTestTable()
    {
        return TestServer.Services
            .GetService<ITestTable>();
    }

    public TestEntity GetTestEntity(string value = "test")
    {
        return new(Guid.NewGuid(), value);
    }

    public void Dispose()
    {
        ((IDisposable) TestServer).Dispose();
        ((IDisposable) TestClient).Dispose();
    }
}