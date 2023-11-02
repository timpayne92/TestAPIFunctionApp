namespace Test_API_Func.Services.Interfaces
{
    public interface ITableStorageService
    {
        Task SendRecordToTableStorage(string tableName, string partitionKey, string rowKey);
    }
}
