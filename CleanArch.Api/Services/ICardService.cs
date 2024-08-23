using CleanArch.Core.Entities;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Entities.RequestModel;

namespace CleanArch.Api.Services
{
    public interface ICardService
    {
        Task<ChargingResponse> ChargingWS(ChargingRequest chargingRequest);
    }
}