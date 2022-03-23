using Azure;
using Azure.Data.Tables;

namespace AzureTables;

interface IEntity : ITableEntity
{
    public Guid Id { get; set; }
}

public class Entity : IEntity
{
    public Guid Id { get; set; }
    public string PartitionKey
    {
        get { return Id.ToString(); }
        set { Id = Guid.Parse(value); }
    }
    public string RowKey
    {
        get { return Id.ToString(); }
        set { Id = Guid.Parse(value); }
    }
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;
    public ETag ETag { get; set; }

    public Entity()
    {
    }
}