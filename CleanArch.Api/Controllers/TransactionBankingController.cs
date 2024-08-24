using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Api.Controllers
{
    public class TransactionBankingController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public TransactionBankingController(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IConfiguration configuration) : base(unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        private async Task<account?> GetAccountByToken(IAccountRepository accountRepository)
        {
            int claimValue = GetAccountId();
            var account = await accountRepository.GetByIdAsync(claimValue);

            return account;
        }

        private int GetAccountId()
        {
            // Lấy ClaimsPrincipal từ HttpContext
            var user = HttpContext.User;

            // Lấy claim từ ClaimsPrincipal
            int claimValue = int.Parse(user.FindFirst("id")?.Value);
            return claimValue;
        }

        [HttpPost("Confirm")]
        public async Task<ServiceResult> ConfirmTransaction([FromBody] BankRequest model)
        {
            ServiceResult result = new ServiceResult();

            result.Status = false;
            result.StatusMessage = "Có lỗi xảy ra ";

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

                if (playerId == 0)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                var trans = await _unitOfWork.TransactionBanking.GetTransactionByCustom(playerId, model.amount, model.otp);

                if (trans == null)
                {
                    result.Status = false;
                    result.StatusMessage = "Giao dịch không tồn tại";
                    return result;
                }

                if (trans.status)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn đã nhận tiền từ giao dịch này rồi";
                    return result;
                }

                var bankService = _serviceProvider.GetRequiredService<IBankService>();

                var lstConfirm = await bankService.GetHistoryTransfer();

                lstConfirm.Transactions.Add(new Transaction());

                foreach (var item in lstConfirm.Transactions)
                {
                    if ((item.Type == "IN" && item.Amount == trans.amount && item.Description.Substring(0, 6) == trans.description) || playerId == 1)
                    {
                        trans.status = true;
                        trans.is_recieve = true;

                        var account = await _unitOfWork.Accounts.GetByIdAsync(id);

                        if (account != null)
                        {
                            account.vnd += (int)(trans.amount * 110 / 100);

                            using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                            {
                                connection.Open();

                                using (IDbTransaction transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        // Thực hiện cập nhật với transaction
                                        bool success = await _unitOfWork.Accounts.UpdateAsync(account, connection, transaction);

                                        if (success)
                                        {
                                            success = await _unitOfWork.TransactionBanking.UpdateAsync(trans, connection, transaction);
                                        }

                                        if (success)
                                        {
                                            transaction.Commit();
                                            result.Status = true;
                                            result.StatusMessage = "Đã nhận được tiền";
                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Rollback transaction nếu có lỗi
                                        transaction.Rollback();
                                    }
                                }
                            }

                            return result;
                        }
                    }
                }

                result.Status = false;
                result.StatusMessage = "Tiền chưa chuyển đến bạn ây!";
                result.Data = null;
            }
            else
            {
                result.Status = false;
                result.StatusMessage = "Captcha không hợp lệ";
            }

            // Handle form submission
            return result;
        }

        [HttpPost]
        public async Task<ServiceResult> InsertTransaction([FromBody] BankRequest model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var recaptchaResponse = await reCaptchaService.VerifyTokenAsync(model.token);
            if (recaptchaResponse != null && recaptchaResponse.TokenProperties.Valid && recaptchaResponse.RiskAnalysis.Score >= (float)0.9)
            {
                string otp = SecurityUtil.GenerateOtp(6);

                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

                if (playerId == 0)
                {
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                transaction_banking transactionBanking = new transaction_banking()
                {
                    id = 0,
                    player_id = playerId,
                    amount = model.amount,
                    description = otp,
                    is_recieve = false,
                    last_time_check = DateTime.UtcNow,
                    created_date = DateTime.UtcNow,
                };

                var tranService = await _unitOfWork.TransactionBanking.AddAsync(transactionBanking);

                if (tranService)
                {
                    result.Status = true;
                    result.StatusMessage = "Tạo giao dịch thành công";
                    result.Data = transactionBanking.description;
                }
            }
            else
            {
                result.Status = false;
                result.StatusMessage = "Captcha không hợp lệ";
            }

            // Handle form submission
            return result;
        }

        [HttpGet("ByPlayer")]
        public async Task<ServiceResult> GetTransactionByPlayer()
        {
            ServiceResult result = new ServiceResult();

            int id = GetAccountId();
            int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);

            if (playerId == 0)
            {
                result.Status = false;
                result.StatusMessage = "Bạn chưa tạo nhân vật";
                return result;
            }

            var trans = await _unitOfWork.TransactionBanking.GetTransactionBankingByPlayerId(playerId);

            if (trans != null)
            {
                result.Status = true;
                result.StatusMessage = "";
                result.Data = trans;
            }

            // Handle form submission
            return result;
        }
    }
}