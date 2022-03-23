using Azure;
using Azure.Data.Tables;

namespace AzureTables;

public interface IEntity<TKey> : ITableEntity
{
    public TKey Id { get; set; }
}

public class Entity : IEntity<Guid>
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