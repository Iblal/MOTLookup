using MOTLookup.Models.Responses;
using MOTLookup.Models.Shared;

namespace MOTLookup.Service.IServices
{
    interface IMOTLookupService
    {
        Task<Result<VehicleResponse>> GetVehicleData(string registration);
    }
}
