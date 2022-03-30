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
    public async Task Can_Get_By_Keys()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = await SaveEntity(table);

        var partitionKey = entity.Id.ToString();
        var rowKey = entity.Id.ToString();
        var insertedEntity = await table.GetAsync(partitionKey, rowKey);

        Assert.NotNull(insertedEntity);
    }

    [Fact]
    public async Task Can_Insert_Entity()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = GetTestEntity();

        table.Insert(entity);
        await table.CommitAsync();
        var insertedEntity = await table.GetAsync(entity.Id);

        Assert.NotNull(insertedEntity);
    }

    [Fact]
    public async Task Can_Insert_Entities_In_Batch()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity1 = GetTestEntity();
        TestEntity entity2 = GetTestEntity();

        table.Insert(entity1);
        table.Insert(entity2);
        await table.CommitAsync();

        var insertedEntity1 = await table.GetAsync(entity1.Id);
        var insertedEntity2 = await table.GetAsync(entity2.Id);
        Assert.NotNull(insertedEntity1);
        Assert.NotNull(insertedEntity2);
    }

    [Fact]
    public async Task Can_Update_Entity()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = await SaveEntity(table);

        var newValue = "Updated !!!";
        entity.Value = newValue;
        table.Update(entity);
        await table.CommitAsync();

        var updatedEntity = await table.GetAsync(entity.Id);
        Assert.Equal(newValue, updatedEntity.Value);
    }

    [Fact]
    public async Task Can_Delete_Entity()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = await SaveEntity(table);

        var insertedEntity = await table.GetAsync(entity.Id);
        table.Delete(entity);
        await table.CommitAsync();

        var deletedEntity = await table.GetAsync(entity.Id);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task Does_Not_Change_Table_When_No_Commited()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        TestEntity entity = GetTestEntity();

        table.Insert(entity);

        var insertedEntity = await table.GetAsync(entity.Id);
        Assert.Null(insertedEntity);
    }

    [Fact]
    public async Task Query_Successfully_With_OData_Filter()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        await SaveEntitiesBatch(table);

        var entities = await table.QueryAsync($"Value ne 'value'");

        Assert.Equal(2, entities.Count());
    }

    [Fact]
    public async Task Query_Successfully_With_Lambda()
    {
        await using var testcontainer = AzuriteBuilder.Build();
        await testcontainer.StartAsync();
        ITestTable? table = GetTestTable();
        await SaveEntitiesBatch(table);

        var entities = await table.QueryAsync(entity => entity.Value != "value");

        Assert.Equal(2, entities.Count());
    }

    private async Task<TestEntity> SaveEntity(ITestTable? table)
    {
        TestEntity entity = GetTestEntity();
        table.Insert(entity);
        await table.CommitAsync();
        return entity;
    }

    private async Task SaveEntitiesBatch(ITestTable? table)
    {
        TestEntity entity1 = GetTestEntity("entity1");
        TestEntity entity2 = GetTestEntity("entity2");
        TestEntity entity3 = GetTestEntity("value");
        table.Insert(entity1);
        table.Insert(entity2);
        table.Insert(entity3);
        await table.CommitAsync();
    }
}