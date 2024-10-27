using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.Responses;
using MOTLookup.Models.Shared;
using MOTLookup.Service.IServices;
using System.Net;

namespace MOTLookup.Service.Services
{
    public class MOTLookupService : IMOTLookupService
    {
        private readonly IMOTApiClient _motApiClient;

        public MOTLookupService(IMOTApiClient motApiClient)
        {
            _motApiClient = motApiClient;
        }

        public async Task<Result<VehicleResponse>> GetVehicleData(string registration)
        {
            var result = new Result<VehicleResponse>();

            try
            {
                var apiResult = await _motApiClient.GetVehicleDataAsync(registration);

                result.StatusCode = apiResult.StatusCode;
                result.Message = apiResult.Message;

                if (apiResult.IsSuccess && apiResult.Data is not null)
                {
                    var apiResponse = apiResult.Data.FirstOrDefault();

                    if (apiResponse is not null)
                    {
                        var lastMotTest = apiResponse.MotTests?.OrderByDescending(m => m.ExpiryDate).FirstOrDefault();

                        result.IsSuccess = true;
                        result.Data = new VehicleResponse(
                            apiResponse.Make,
                            apiResponse.Model,
                            apiResponse.PrimaryColour,
                            DateTime.Parse(lastMotTest.ExpiryDate),
                            lastMotTest.OdometerValue
                        );
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Vehicle data is missing from the response.";
                    }
                }
                else
                {
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Message = "An unexpected error occurred while processing the data.";
            }

            return result;
        }
    }
}
