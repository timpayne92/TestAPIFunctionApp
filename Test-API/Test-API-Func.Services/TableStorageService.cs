using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Test_API_Func.Services.Interfaces;

namespace Test_API_Func.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<TableStorageService> _logger;

        public TableStorageService(TableServiceClient tableServiceClient, ILogger<TableStorageService> logger)
        {
            _tableServiceClient = tableServiceClient;
            _logger = logger;
        }

        public async Task SendRecordToTableStorage(string tableName, string partitionKey, string rowKey)
        {
            try
            {
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                // Hardcoded data for testing purposes
                var tableEntity = new TableEntity(partitionKey, rowKey)
                {
                    { "FirstName","Shane" },
                    { "LastName", "Watson" },
                    { "Country", "Australia" },
                    { "Occupation", "Athlete" }
                };

                await tableClient.AddEntityAsync(tableEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendRecordToTableStorage)}: {ex.Message}");
                throw;
            }
        }
    }
}
