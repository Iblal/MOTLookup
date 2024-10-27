using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MOTLookup.Models.ApiResponse;
using System.Net;
using System.Text.Json;

public class MOTApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<MOTApiClient>> _loggerMock;
    private readonly MOTApiClient _motApiClient;

    public MOTApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://test.com/")
        };
        _loggerMock = new Mock<ILogger<MOTApiClient>>();
        _motApiClient = new MOTApiClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetVehicleDataAsync_ReturnsSuccessResult_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var registration = "TEST123";
        var apiResponse = new List<MOTAPIResponse>
        {
            new MOTAPIResponse { Registration = registration, Make = "TestMake", Model = "TestModel" }
        };
        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _motApiClient.GetVehicleDataAsync(registration);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(apiResponse.Count, result.Data.Count);
        Assert.Equal(apiResponse[0].Registration, result.Data[0].Registration);
    }

    [Fact]
    public async Task GetVehicleDataAsync_ReturnsErrorResult_WhenApiResponseIsNotFound()
    {
        // Arrange
        var registration = "TEST123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        // Act
        var result = await _motApiClient.GetVehicleDataAsync(registration);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("Vehicle not found. Please check the registration number.", result.Message);
    }

    [Fact]
    public async Task GetVehicleDataAsync_ReturnsErrorResult_WhenExceptionIsThrown()
    {
        // Arrange
        var registration = "TEST123";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _motApiClient.GetVehicleDataAsync(registration);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("An unexpected error occurred. Please try again later.", result.Message);
    }
}
