using Azure;

namespace AzureTables.Program;

public class Role : IEntity<string>
{
    public string Id
    {
        get { return RowKey; }
        set { RowKey = value; }
    }
    public string PartitionKey { get; set; } = "Roles";
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Role()
    {
    }

    public Role(string name)
    {
        Id = name;
    }
}

[Table("Roles")]
public class RolesTable : Table<Role, string>
{
    public RolesTable(AzureStorageSettings storageSettings)
        : base(storageSettings)
    {
    }
}