using Microsoft.Extensions.Logging;
using Moq;
using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Shared;
using MOTLookup.Service.Services;
using System.Net;

namespace MOTLookup.Tests.Services
{
    public class MOTLookupServiceTests
    {
        private readonly Mock<IMOTApiClient> _motApiClientMock;
        private readonly Mock<ILogger<MOTLookupService>> _loggerMock;
        private readonly MOTLookupService _motLookupService;

        public MOTLookupServiceTests()
        {
            _motApiClientMock = new Mock<IMOTApiClient>();
            _loggerMock = new Mock<ILogger<MOTLookupService>>();
            _motLookupService = new MOTLookupService(_motApiClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsSuccessResult_WhenApiReturnsValidData()
        {
            // Arrange
            var registration = "ABC123";
            var motApiResponse = new MOTAPIResponse
            {
                Make = "Toyota",
                Model = "Corolla",
                PrimaryColour = "Red",
                MotTests = new List<MOTTestApiResponse>
                {
                    new MOTTestApiResponse { ExpiryDate = "2023-12-31", OdometerValue = "15000" }
                }
            };
            var apiResult = new Result<List<MOTAPIResponse>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = new List<MOTAPIResponse> { motApiResponse }
            };

            _motApiClientMock.Setup(client => client.GetVehicleDataAsync(registration))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Toyota", result.Data.Make);
            Assert.Equal("Corolla", result.Data.Model);
            Assert.Equal("Red", result.Data.Colour);
            Assert.Equal("15000", result.Data.MileageAtLastMot);
            Assert.Equal(new DateTime(2023, 12, 31), result.Data.MotExpiryDate);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsFailureResult_WhenApiReturnsNoData()
        {
            // Arrange
            var registration = "ABC123";
            var apiResult = new Result<List<MOTAPIResponse>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = new List<MOTAPIResponse>()
            };

            _motApiClientMock.Setup(client => client.GetVehicleDataAsync(registration))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Vehicle data is missing from the response.", result.Message);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsFailureResult_WhenApiCallFails()
        {
            // Arrange
            var registration = "ABC123";
            var apiResult = new Result<List<MOTAPIResponse>>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Bad Request"
            };

            _motApiClientMock.Setup(client => client.GetVehicleDataAsync(registration))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Bad Request", result.Message);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsFailureResult_WhenExceptionIsThrown()
        {
            // Arrange
            var registration = "ABC123";

            _motApiClientMock.Setup(client => client.GetVehicleDataAsync(registration))
                .ThrowsAsync(new Exception("API error"));

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("An unexpected error occurred while processing the data.", result.Message);
        }
    }
}
