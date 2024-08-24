using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface ICardService
    {
        Task<ChargingResponse> ChargingWS(ChargingRequest chargingRequest);
    }
}