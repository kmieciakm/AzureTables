using Azure.Data.Tables;

namespace AzureTables;

internal class Transaction : IDisposable
{
    private TableClient _TableClient { get; set; }
    private readonly List<TableTransactionAction> _transactionActions = new();

    public Transaction(TableClient tableClient)
    {
        _TableClient = tableClient;
    }

    public void AddAction(TableTransactionAction transactionAction)
    {
        _transactionActions.Add(transactionAction);
    }

    public async Task CommitAsync()
    {
        if (_transactionActions.Count > 0)
        {
            await _TableClient.SubmitTransactionAsync(_transactionActions);
            _transactionActions.Clear();
        }
    }

    public void Rollback()
    {
        _transactionActions.Clear();
    }

    public void Dispose()
    {
        Rollback();
    }
}