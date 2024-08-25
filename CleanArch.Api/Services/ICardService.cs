using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;

namespace CleanArch.Api.Services
{
    public interface ICardService
    {
        Task<ChargingResponse> ChargingWS(ChargingRequest chargingRequest, IUnitOfWork unitOfWork, int accountId);
        Task HandleCallback(CallbackRequest callbackRequest, IUnitOfWork unitOfWork);
        Task<ChargingResponse> CheckTransaction(CheckRequestModel checkRequest, IUnitOfWork unitOfWork, int accountId);
    }
}