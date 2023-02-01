using Azure;
using Azure.Data.Tables;

namespace MealBot.Models;

public class BaseTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "MealBot";
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}