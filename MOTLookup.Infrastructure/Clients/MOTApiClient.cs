using Microsoft.Extensions.Logging;
using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Shared;
using System.Net;
using System.Text.Json;

public class MOTApiClient : IMOTApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MOTApiClient> _logger;

    public MOTApiClient(HttpClient httpClient, ILogger<MOTApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<List<MOTAPIResponse>>> GetVehicleDataAsync(string registration)
    {
        var result = new Result<List<MOTAPIResponse>>();

        try
        {
            var response = await _httpClient.GetAsync($"?registration={registration}");

            result.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponses = JsonSerializer.Deserialize<List<MOTAPIResponse>>(content, options);

                result.IsSuccess = true;
                result.Data = apiResponses;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = GetErrorMessage(response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching data for registration {Registration}", registration);
            result.IsSuccess = false;
            result.StatusCode = HttpStatusCode.InternalServerError;
            result.Message = "An unexpected error occurred. Please try again later.";
        }

        return result;
    }

    private string GetErrorMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Invalid request. Please check the registration number and try again.",
            HttpStatusCode.Unauthorized => "Access to the MOT service is unauthorized.",
            HttpStatusCode.NotFound => "Vehicle not found. Please check the registration number.",
            HttpStatusCode.UnsupportedMediaType => "Unsupported media type in the request.",
            HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please try again later.",
            HttpStatusCode.InternalServerError or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout => "The MOT service is currently unavailable. Please try again later.",
            _ => "An unexpected error occurred. Please try again later.",
        };
    }
}
