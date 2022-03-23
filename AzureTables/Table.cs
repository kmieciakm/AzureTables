using Azure.Data.Tables;

namespace AzureTables;

public class TableAttribute : Attribute
{
    public string Name { get; set; }

    public TableAttribute(string name)
    {
        Name = name;
    }
}

public interface ITable<E, TKey> where E : IEntity<TKey>
{
    Task<E?> QueryAsync(TKey id);
    void Insert(E entity);
    void Delete(E entity);
    void Update(E entity);
    void Rollback();
    Task CommitAsync();
}

public abstract class Table<E, TKey> : ITable<E, TKey> where E : class, IEntity<TKey>, new()
{
    private readonly AzureStorageSettings _storageSettings;
    private Transaction _transaction { get; }
    private TableClient _tableClient { get; }

    public Table(AzureStorageSettings storageSettings)
    {
        _storageSettings = storageSettings;
        _tableClient = GetTableClient();
        _transaction = new Transaction(_tableClient);
    }

    private TableClient GetTableClient()
    {
        TableAttribute? tableAttribute = GetType()
            .GetCustomAttributes(typeof(TableAttribute), false)
            .FirstOrDefault() as TableAttribute;

        if (tableAttribute is null)
        {
            throw new AzureTableException(
                $"Table name is not specified for type {GetType()}. Use {nameof(TableAttribute)} to provide a table name.");
        }
        else
        {
            TableServiceClient tableStorage = new(_storageSettings.ConnectionString);
            tableStorage.CreateTableIfNotExists(tableAttribute.Name);
            return new TableClient(
                    _storageSettings.ConnectionString,
                    tableAttribute.Name
                );
        }
    }

    public void Delete(E entity)
    {
        TableTransactionAction action = new(
            TableTransactionActionType.Delete, entity); 
        _transaction.AddAction(action);
    }

    public void Insert(E entity)
    {
        TableTransactionAction action = new(
            TableTransactionActionType.Add, entity);
        _transaction.AddAction(action);
    }

    public async Task<E?> QueryAsync(TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        var response = await _tableClient
            .GetEntityAsync<E>(id.ToString(), id.ToString());
        return response.Value;
    }

    public void Update(E entity)
    {
        TableTransactionAction action = new(
            TableTransactionActionType.UpdateMerge, entity);
        _transaction.AddAction(action);
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }
}