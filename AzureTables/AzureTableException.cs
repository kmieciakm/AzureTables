namespace AzureTables;

internal class AzureTableException : Exception
{
    public string TableName { get; set; } = string.Empty;

    public AzureTableException(string tableName = "")
    {
        TableName = tableName;
    }

    public AzureTableException(string? message, string tableName = "")
        : base(message)
    {
        TableName = tableName;
    }

    public AzureTableException(string? message, Exception? innerException, string tableName = "")
        : base(message, innerException)
    {
        TableName = tableName;
    }
}