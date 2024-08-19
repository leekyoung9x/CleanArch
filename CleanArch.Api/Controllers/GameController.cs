using CleanArch.Api.Models;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CleanArch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly GlobalMessageHandler _gMessage;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize ContactController by injecting an object type of IUnitOfWork
        /// </summary>
        public GameController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;

            _gMessage = GlobalMessageHandler.gI();

            Session_ME.gI().setHandler(_gMessage);

            GameCanvas.readMessenge = new ReadMessenge();

            //gMessage.globalLogicHandler.OnConnect += Connection_OnConnect;
            //gMessage.OnMessage += Connection_OnMessage;
        }

        #endregion

        [HttpGet("Connect")]
        public async Task<ApiResponse<List<Contact>>> GetAll()
        {
            var apiResponse = new ApiResponse<List<Contact>>();

            try
            {
                GameCanvas.connect();

                apiResponse.Success = true;
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }

        [HttpGet("gold")]
        public async Task<ApiResponse<List<Contact>>> AddGold()
        {
            var apiResponse = new ApiResponse<List<Contact>>();

            try
            {
                GlobalService.gI().addGoldPlayer(1, 1000);
                apiResponse.Success = true;
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
            }

            return apiResponse;
        }
    }
}
