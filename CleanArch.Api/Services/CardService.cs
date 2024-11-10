using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.Enumeration;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Utils;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.Json;

namespace CleanArch.Api.Services
{
    public class CardService : ICardService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CardService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ChargingResponse> ChargingWS(ChargingRequest chargingRequest, IUnitOfWork unitOfWork, int accountId)
        {

            var baseURL = _configuration["Card:BaseURL"];
            var partnerID = _configuration["Card:PartnerID"];
            var partnerKey = _configuration["Card:PartnerKey"];
            var httpClient = _httpClientFactory.CreateClient();

            Random random = new Random();
            chargingRequest.RequestId = random.Next(0, Int32.MaxValue).ToString();

            chargingRequest.Sign = HashHelper.ComputeMd5Hash(
                partnerKey + chargingRequest.Code + chargingRequest.Serial
             );

            // Tạo nội dung yêu cầu
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("telco", chargingRequest.Telco),
                new KeyValuePair<string, string>("code", chargingRequest.Code),
                new KeyValuePair<string, string>("serial", chargingRequest.Serial),
                new KeyValuePair<string, string>("amount", chargingRequest.Amount),
                new KeyValuePair<string, string>("request_id", chargingRequest.RequestId),
                new KeyValuePair<string, string>("partner_id", partnerID),
                new KeyValuePair<string, string>("sign", chargingRequest.Sign),
                new KeyValuePair<string, string>("command", "charging")
            });

            var response = await httpClient.PostAsync($"{baseURL}/chargingws/v2", content);
            var jsonString = await response.Content.ReadAsStringAsync();

            // Deserialize JSON thành ReCaptchaResponse
            var chargingResponse = JsonSerializer.Deserialize<ChargingResponse>(jsonString);
            await HandleCard(chargingRequest, chargingResponse, unitOfWork, accountId);

            return chargingResponse;
        }

        public async Task<ChargingResponse> CheckTransaction(CheckRequestModel checkRequest, IUnitOfWork unitOfWork, int accountId)
        {
            var chargingResponse = new ChargingResponse();

            
            var tran = (await unitOfWork.TransactionCard.GetByIdAsync("request_id", checkRequest.request_id)).FirstOrDefault();

            if (tran != null)
            {
                int playerId = await unitOfWork.Accounts.GetPlayerIdByAccountId(accountId);

                if (playerId != tran.player_id)
                {
                    chargingResponse.Status = 100;
                    return chargingResponse;
                }

                if (tran.status == (int)CardType.Success || tran.status == (int)CardType.SuccessHalf)
                {
                    chargingResponse.Status = 100;
                    return chargingResponse;
                }

                var baseURL = _configuration["Card:BaseURL"];
                var partnerID = _configuration["Card:PartnerID"];
                var partnerKey = _configuration["Card:PartnerKey"];
                var httpClient = _httpClientFactory.CreateClient();

                string sign = HashHelper.ComputeMd5Hash(
                    partnerKey + tran.code + tran.seri
                 );

                // Tạo nội dung yêu cầu
                var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("telco", tran.card_type),
                    new KeyValuePair<string, string>("code", tran.code),
                    new KeyValuePair<string, string>("serial", tran.seri),
                    new KeyValuePair<string, string>("amount", tran.amount.ToString()),
                    new KeyValuePair<string, string>("request_id", tran.request_id),
                    new KeyValuePair<string, string>("partner_id", partnerID),
                    new KeyValuePair<string, string>("sign", sign),
                    new KeyValuePair<string, string>("command", "check")
                });

                var response = await httpClient.PostAsync($"{baseURL}/chargingws/v2", content);
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize JSON thành ReCaptchaResponse
                if (!string.IsNullOrEmpty(jsonString))
                {
                    chargingResponse = JsonSerializer.Deserialize<ChargingResponse>(jsonString);
                }
                await HandleCard(tran, chargingResponse, unitOfWork, accountId);
            }

            return chargingResponse;
        }

        public async Task HandleCallback(CallbackRequest callbackRequest, IUnitOfWork unitOfWork)
        {
            var partnerKey = _configuration["Card:PartnerKey"];

            string callBackKey = HashHelper.ComputeMd5Hash(
                partnerKey + callbackRequest.code + callbackRequest.serial
            );

            if (callBackKey != callbackRequest.callback_sign)
            {
                return;
            }

            await HandleCard(callbackRequest, unitOfWork);
        }

        private async Task HandleCard(CallbackRequest callbackRequest, IUnitOfWork unitOfWork)
        {
            account? accountDb = new account();
            bool isUpdateAccount = callbackRequest != null && (callbackRequest.status == (int)CardType.Success || callbackRequest.status == (int)CardType.SuccessHalf);
            var tran = (await unitOfWork.TransactionCard.GetByIdAsync("request_id", callbackRequest.request_id)).FirstOrDefault();

            if (tran != null && (tran.status != (int)CardType.Success || tran.status != (int)CardType.SuccessHalf))
            {
                int accountId = await unitOfWork.Accounts.GetAccountIdByPlayerId(tran.player_id);

                tran.status = callbackRequest != null ? (int)callbackRequest.status : -99;

                if (isUpdateAccount)
                {
                    accountDb = await unitOfWork.Accounts.GetByIdAsync(accountId);
                    tran.amount_real = callbackRequest?.value ?? 0;

                    if (accountDb != null)
                    {
                        accountDb.vnd += tran.amount_real;

                        bool haveEventPoint = false;
                        haveEventPoint = bool.TryParse(_configuration["Event:HaveEvent"], out haveEventPoint);

                        if (haveEventPoint)
                        {
                            accountDb.pointNap += (int)tran.amount_real / 1000;
                        }
                    }
                }

                using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Thực hiện cập nhật với transaction
                            bool success = await unitOfWork.TransactionCard.UpdateAsync(tran, connection, transaction);

                            if (success && isUpdateAccount && accountDb != null)
                            {
                                success = await unitOfWork.Accounts.UpdateAsync(accountDb, connection, transaction);
                            }

                            if (success)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            // Rollback transaction nếu có lỗi
                            transaction.Rollback();
                        }
                    }
                }
            }
        }

        private async Task HandleCard(transaction_card transactionCard, ChargingResponse? chargingResponse, IUnitOfWork unitOfWork, int accountId)
        {
            if (chargingResponse != null && (chargingResponse.Status == (int)CardType.Success ||
                chargingResponse.Status == (int)CardType.SuccessHalf || chargingResponse.Status == (int)CardType.Pending))
            {
                account? accountDb = new account();
                bool isUpdateAccount = chargingResponse != null && (chargingResponse.Status == (int)CardType.Success || chargingResponse.Status == (int)CardType.SuccessHalf);
                int playerId = await unitOfWork.Accounts.GetPlayerIdByAccountId(accountId);

                transactionCard.status = chargingResponse != null && chargingResponse.Status != null ? (int)chargingResponse.Status : -99;

                if (isUpdateAccount)
                {
                    accountDb = await unitOfWork.Accounts.GetByIdAsync(accountId);
                    transactionCard.amount_real = chargingResponse.Value ?? 0;

                    if (accountDb != null)
                    {
                        accountDb.vnd += transactionCard.amount_real;

                        bool haveEventPoint = false;
                        haveEventPoint = bool.TryParse(_configuration["Event:HaveEvent"], out haveEventPoint);

                        if (haveEventPoint)
                        {
                            accountDb.pointNap += (int)transactionCard.amount_real / 1000;
                        }
                    }
                }

                using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Thực hiện cập nhật với transaction
                            bool success = await unitOfWork.TransactionCard.UpdateAsync(transactionCard, connection, transaction);

                            if (success && isUpdateAccount && accountDb != null)
                            {
                                success = await unitOfWork.Accounts.UpdateAsync(accountDb, connection, transaction);
                            }

                            if (success)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            // Rollback transaction nếu có lỗi
                            transaction.Rollback();
                        }
                    }
                }
            }
        }

        private async Task HandleCard(ChargingRequest chargingRequest, ChargingResponse? chargingResponse, IUnitOfWork unitOfWork, int accountId)
        {
            if (chargingResponse != null && (chargingResponse.Status == (int)CardType.Success ||
                chargingResponse.Status == (int)CardType.SuccessHalf || chargingResponse.Status == (int)CardType.Pending))
            {
                transaction_card itemDb = new transaction_card();
                account? accountDb = new account();
                bool isUpdateAccount = chargingResponse != null && (chargingResponse.Status == (int)CardType.Success || chargingResponse.Status == (int)CardType.SuccessHalf);
                int playerId = await unitOfWork.Accounts.GetPlayerIdByAccountId(accountId);

                itemDb.request_id = chargingRequest.RequestId;
                itemDb.player_id = playerId;
                itemDb.amount = int.Parse(chargingRequest.Amount);
                itemDb.seri = chargingRequest.Serial;
                itemDb.code = chargingRequest.Code;
                itemDb.card_type = chargingRequest.Telco;
                itemDb.time = DateTime.Now;

                if (chargingRequest != null)
                {
                    itemDb.status = chargingResponse != null && chargingResponse.Status != null ? (int)chargingResponse.Status : -99;
                }

                if (isUpdateAccount)
                {
                    accountDb = await unitOfWork.Accounts.GetByIdAsync(accountId);
                    itemDb.amount_real = chargingResponse.Value ?? 0;

                    if (accountDb != null)
                    {
                        accountDb.vnd += itemDb.amount_real;

                        bool haveEventPoint = false;
                        haveEventPoint = bool.TryParse(_configuration["Event:HaveEvent"], out haveEventPoint);

                        if (haveEventPoint)
                        {
                            accountDb.pointNap += (int)itemDb.amount_real / 1000;
                        }
                    }
                }

                using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Thực hiện cập nhật với transaction
                            bool success = await unitOfWork.TransactionCard.AddAsync(itemDb, connection, transaction);

                            if (success && isUpdateAccount && accountDb != null)
                            {
                                success = await unitOfWork.Accounts.UpdateAsync(accountDb, connection, transaction);
                            }

                            if (success)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            // Rollback transaction nếu có lỗi
                            transaction.Rollback();
                        }
                    }
                }
            }
        }
    }
}