using Azure.Data.Tables;
using System.Linq.Expressions;

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
    Task<E?> GetAsync(string partitionKey, string rowKey);
    Task<E?> GetAsync(TKey id);
    Task<IEnumerable<E>> QueryAsync(string filter);
    Task<IEnumerable<E>> QueryAsync(Expression<Func<E, bool>> filter);
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

    public async Task<E?> GetAsync(string partitionKey, string rowKey)
    {
        ArgumentNullException.ThrowIfNull(partitionKey);
        ArgumentNullException.ThrowIfNull(rowKey);

        var response = await _tableClient
            .GetEntityAsync<E>(partitionKey, rowKey);
        return response.Value;
    }

    public async Task<E?> GetAsync(TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        var query = _tableClient.QueryAsync<E>($"Id eq '{id}'");
        await foreach (var entity in query)
        {
            return entity;
        }
        return null;
    }

    public async Task<IEnumerable<E>> QueryAsync(Expression<Func<E, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        List<E> result = new();
        await foreach(var entity in _tableClient.QueryAsync(filter))
        {
            result.Add(entity);
        }
        return result;
    }

    public async Task<IEnumerable<E>> QueryAsync(string filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        List<E> result = new();
        await foreach (var entity in _tableClient.QueryAsync<E>(filter))
        {
            result.Add(entity);
        }
        return result;
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