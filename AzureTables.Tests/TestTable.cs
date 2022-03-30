namespace AzureTables.Tests;


public class TestEntity : Entity
{
    public TestEntity() { }

    public TestEntity(Guid id)
    {
        Id = id;
    }

    public TestEntity(Guid id, string value)
        : this(id)
    {
        Value = value;
    }

    public string? Value { get; set; }
}

public interface ITestTable : ITable<TestEntity, Guid>
{
}

[Table("TestTable")]
public class TestTable : Table<TestEntity, Guid>, ITestTable
{
    public TestTable(AzureStorageSettings storageSettings)
        : base(storageSettings)
    {
    }
}