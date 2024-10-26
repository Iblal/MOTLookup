using MOTLookup.Models.ApiResponse;
using MOTLookup.Models.Shared;

namespace MOTLookup.Infrastructure.IClients
{
    public interface IMOTApiClient
    {
        Task<Result<List<MOTAPIResponse>>> GetVehicleDataAsync(string registration);
    }
}
