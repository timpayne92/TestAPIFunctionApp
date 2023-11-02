using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;
using Test_API;
using Test_API_Func.Common;
using Test_API_Func.Common.Models;
using Test_API_Func.Services.Interfaces;

namespace Test_API_Func.Functions
{
    public class CreateEmployee
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IValidator<Employee> _validator;
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public CreateEmployee(ITableStorageService tableStorageService,
                              IValidator<Employee> validator,
                              IOptions<Settings> options,
                              ILoggerFactory loggerFactory)
        {
            _tableStorageService = tableStorageService;
            _validator = validator;
            _settings = options.Value;
            _logger = loggerFactory.CreateLogger<CreateEmployee>();
        }

        [Function("CreateEmployee")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Test Employee API" })]
        [OpenApiSecurity("function_key",
            SecuritySchemeType.ApiKey,
            Name = "x-functions-key",
            In = OpenApiSecurityLocationType.Header)]
        [OpenApiRequestBody("application/json", typeof(Employee))]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "Success")]
        [OpenApiResponseWithBody(HttpStatusCode.UnprocessableEntity,
            "application/json",
            typeof(ErrorResponse),
            Description = "Validation Errors")]
        [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        [OpenApiResponseWithoutBody(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                var request = await new StreamReader(req.Body).ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var employee = JsonSerializer.Deserialize<Employee>(request, options);

                var employeeModelValidationResult = await _validator.ValidateAsync(employee);

                if (!employeeModelValidationResult.IsValid)
                {
                    var validationErrors = employeeModelValidationResult.Errors.Select(e => new
                    {
                        e.PropertyName,
                        e.ErrorMessage
                    });

                    var response = req.CreateResponse(HttpStatusCode.UnprocessableEntity);

                    await response.WriteAsJsonAsync(validationErrors, HttpStatusCode.UnprocessableEntity);

                    return response;
                }

                await _tableStorageService.SendRecordToTableStorage(
                    _settings.StorageAccountTableName, employee.Id, Guid.NewGuid().ToString());
                _logger.LogInformation("Added record for the employee in table storage.");

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
