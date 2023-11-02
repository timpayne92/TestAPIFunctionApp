using AutoFixture;
using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Test_API_Func.Services;
using Xunit;

namespace Test_API_Func.Service.UnitTests
{
    public class TableStorageServiceShould
    {
        private readonly Mock<TableServiceClient> _tableServiceClient;
        private readonly Mock<TableClient> _tableClientMock;
        private readonly Mock<ILogger<TableStorageService>> _loggerMock;

        private readonly TableStorageService _tableStorageServiceSut;

        public TableStorageServiceShould()
        {
            _tableServiceClient = new Mock<TableServiceClient>();
            _tableClientMock = new Mock<TableClient>();
            _loggerMock = new Mock<ILogger<TableStorageService>>();

            _tableStorageServiceSut = new TableStorageService(_tableServiceClient.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SendRecordToTableStorage()
        {
            // ARRANGE
            var fixture = new Fixture();
            var tableEntity = fixture.Create<TableEntity>();
            var response = new Mock<Response>();

            _tableServiceClient
                .Setup(x => x.GetTableClient(It.IsAny<string>()))
                .Returns(_tableClientMock.Object);

            _tableClientMock
                .Setup(x => x.AddEntityAsync(tableEntity, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(response.Object));

            // ACT
            Func<Task> tableServiceAction = async () => await _tableStorageServiceSut.SendRecordToTableStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // ASSERT
            await tableServiceAction.Should().NotThrowAsync<Exception>();
            _tableClientMock.Verify(x => x.AddEntityAsync(It.IsAny<TableEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToTableStorage()
        {
            // ARRANGE
            var fixture = new Fixture();
            var tableEntity = fixture.Create<TableEntity>();
            var response = new Mock<Response>();

            _tableServiceClient
                .Setup(x => x.GetTableClient(It.IsAny<string>()))
                .Returns(_tableClientMock.Object);

            _tableClientMock
                .Setup(x => x.AddEntityAsync(tableEntity, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> tableServiceAction = async () => await _tableStorageServiceSut.SendRecordToTableStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // ASSERT
            await tableServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToTableStorage: Mock Failure"));
        }
    }
}
