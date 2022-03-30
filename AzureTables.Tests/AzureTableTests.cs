using Xunit;

namespace AzureTables.Tests;

public class AzureTableTests : TestBase
{
    [Fact]
    public void Should_Create_Test_Server()
    {
        Assert.NotNull(TestServer);
    }

    [Fact]
    public void Should_Create_Test_Http_Client()
    {
        Assert.NotNull(TestClient);
    }

    [Fact]
    public void Can_Retrive_Local_Storage_Settings()
    {
        AzureStorageSettings? settings = GetStorageSettings();
        Assert.NotNull(settings?.ConnectionString);
    }

    [Fact]
    public async Task Can_Connect_To_Azurite_Container()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();

        ITestTable? table = GetTestTable();
        Assert.NotNull(table);
    }

    [Fact]
    public async Task Can_Insert_To_Table()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = GetTestEntity();

        table.Insert(entity);
        await table.CommitAsync();

        Assert.NotNull(table.GetAsync(entity.Id));
    }
}