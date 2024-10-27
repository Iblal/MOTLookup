using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Responses;
using MOTLookup.Models.Shared;
using MOTLookup.Service.Services;
using System.Net;

namespace MOTLookup.Tests.Services
{
    public class MOTLookupServiceTests
    {
        private readonly Mock<IMOTApiClient> _motApiClientMock;
        private readonly Mock<ILogger<MOTLookupService>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly MOTLookupService _motLookupService;

        public MOTLookupServiceTests()
        {
            _motApiClientMock = new Mock<IMOTApiClient>();
            _loggerMock = new Mock<ILogger<MOTLookupService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _motLookupService = new MOTLookupService(_motApiClientMock.Object, _loggerMock.Object, _memoryCache);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsSuccessResult_WhenApiReturnsValidData()
        {
            // Arrange
            var registration = "VALID";
            var motApiResponse = new MOTAPIResponse
            {
                Make = "Toyota",
                Model = "Yaris",
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
            Assert.Equal("Yaris", result.Data.Model);
            Assert.Equal("Red", result.Data.Colour);
            Assert.Equal("15000", result.Data.MileageAtLastMot);
            Assert.Equal(new DateTime(2023, 12, 31), result.Data.MotExpiryDate);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsFailureResult_WhenApiReturnsNoData()
        {
            // Arrange
            var registration = "INVALID";
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
            Assert.Equal("Vehicle data could not be fetched.", result.Message);
        }

        [Fact]
        public async Task GetVehicleData_ReturnsFailureResult_WhenApiCallFails()
        {
            // Arrange
            var registration = "VALID";
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
            var registration = "INVALID";

            _motApiClientMock.Setup(client => client.GetVehicleDataAsync(registration))
                .ThrowsAsync(new Exception("API error"));

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Equal("An unexpected error occurred while processing the data.", result.Message);
        }

        [Fact]
        public async Task GetVehicleData_ShouldReturnCachedData_WhenCacheIsHit()
        {
            // Arrange
            var registration = "VALID";
            var cachedVehicle = new VehicleResponse("Toyota", "Yaris", "Red", DateTime.Now, "10000");
            _memoryCache.Set(registration, cachedVehicle);

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(cachedVehicle, result.Data);
            _motApiClientMock.Verify(x => x.GetVehicleDataAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetVehicleData_ShouldReturnApiData_WhenCacheIsMissed()
        {
            // Arrange
            var registration = "VALID";
            var apiResponse = new MOTAPIResponse
            {
                Make = "Toyota",
                Model = "Yaris",
                PrimaryColour = "Yaris",
                MotTests = new List<MOTTestApiResponse>
            {
                new MOTTestApiResponse { ExpiryDate = DateTime.Now.AddYears(1).ToString(), OdometerValue = "10000" }
            }
            };
            var apiResult = new Result<List<MOTAPIResponse>>
            {
                IsSuccess = true,
                Data = new List<MOTAPIResponse> { apiResponse }
            };
            _motApiClientMock.Setup(x => x.GetVehicleDataAsync(registration)).ReturnsAsync(apiResult);

            // Act
            var result = await _motLookupService.GetVehicleData(registration);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(apiResponse.Make, result.Data.Make);
            Assert.Equal(apiResponse.Model, result.Data.Model);
            Assert.Equal(apiResponse.PrimaryColour, result.Data.Colour);
            _motApiClientMock.Verify(x => x.GetVehicleDataAsync(registration), Times.Once);
        }
    }
}
