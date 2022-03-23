namespace AzureTables.Program;

public class User : Entity
{
    public User() { }

    public User(Guid id)
    {
        Id = id;
    }

    public User(Guid id, string firstname, string lastname)
        : this(id)
    {
        Firstname = firstname;
        Lastname = lastname;
    }

    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
}

[Table("Users")]
public class UserTable : Table<User>
{
    public UserTable(AzureStorageSettings storageSettings)
        : base(storageSettings)
    {
    }
}