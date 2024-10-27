using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Responses;
using MOTLookup.Models.Shared;
using MOTLookup.Service.IServices;
using System.Net;

namespace MOTLookup.Service.Services
{
    public class MOTLookupService : IMOTLookupService
    {
        private readonly IMOTApiClient _motApiClient;
        private readonly ILogger<MOTLookupService> _logger;
        private readonly IMemoryCache _cache;

        public MOTLookupService(IMOTApiClient motApiClient, ILogger<MOTLookupService> logger, IMemoryCache cache)
        {
            _motApiClient = motApiClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Result<VehicleResponse>> GetVehicleData(string registration)
        {
            var result = new Result<VehicleResponse>();

            try
            {
                if (VehicleDataIsCached(registration, out var cachedVehicle))
                {
                    result.IsSuccess = true;
                    result.Data = cachedVehicle;

                    return result;
                }

                var apiResult = await _motApiClient.GetVehicleDataAsync(registration);

                result.StatusCode = apiResult.StatusCode;
                result.Message = apiResult.Message;

                if (apiResult.IsSuccess && apiResult.Data is not null)
                {
                    var apiResponse = apiResult.Data.FirstOrDefault();

                    if (apiResponse is not null)
                    {
                        result.Data = CreateVehicleResponse(apiResponse);
                        result.IsSuccess = true;
                        CacheVehicleData(registration, result.Data);
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Vehicle data could not be fetched.";
                    }
                }
                else
                {
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in lookup service while processing data for registration {Registration}", registration);
                result.IsSuccess = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Message = "An unexpected error occurred while processing the data.";
            }

            return result;
        }

        private bool VehicleDataIsCached(string registration, out VehicleResponse cachedVehicle)
        {
            if (_cache.TryGetValue(registration, out cachedVehicle))
            {
                _logger.LogInformation("Cache hit for registration: {Registration}", registration);
                return true;
            }
            return false;
        }

        private VehicleResponse CreateVehicleResponse(MOTAPIResponse apiResponse)
        {
            var lastMotTest = apiResponse.MotTests?.OrderByDescending(m => m.ExpiryDate).FirstOrDefault();
            return new VehicleResponse(
                apiResponse.Make,
                apiResponse.Model,
                apiResponse.PrimaryColour,
                DateTime.Parse(lastMotTest.ExpiryDate),
                lastMotTest.OdometerValue
            );
        }

        private void CacheVehicleData(string registration, VehicleResponse vehicleData)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            _cache.Set(registration, vehicleData, cacheEntryOptions);
        }
    }
}
