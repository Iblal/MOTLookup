using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Shared;

namespace MOTLookup.Infrastructure.IClients
{
    internal interface IMOTApiClient
    {
        Task<Result<List<MOTAPIResponse>>> GetVehicleDataAsync(string registration);
    }
}
