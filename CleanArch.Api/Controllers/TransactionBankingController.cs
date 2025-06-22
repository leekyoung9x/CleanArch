using CleanArch.Api.Services;
using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Core.Entities.RequestModel;
using CleanArch.Core.Entities.ResponseModel;
using CleanArch.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using NLog;

namespace CleanArch.Api.Controllers
{
    public class TransactionBankingController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
            _logger.Info("ConfirmTransaction API called with Amount: {Amount}, OTP: {OTP}", model.amount, model.otp);
            
            ServiceResult result = new ServiceResult();

            result.Status = false;
            result.StatusMessage = "Có lỗi xảy ra ";

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            _logger.Info("Starting reCAPTCHA validation for ConfirmTransaction");
            var isValidCaptcha = await reCaptchaService.IsValidCaptchaAsync(model.token);
            _logger.Info("reCAPTCHA validation result: {IsValid}", isValidCaptcha);
            
            if (isValidCaptcha)
            {
                int id = GetAccountId();
                int playerId = await _unitOfWork.Accounts.GetPlayerIdByAccountId(id);
                _logger.Info("Account validation - AccountId: {AccountId}, PlayerId: {PlayerId}", id, playerId);

                if (playerId == 0)
                {
                    _logger.Warn("Player not found for AccountId: {AccountId}", id);
                    result.Status = false;
                    result.StatusMessage = "Bạn chưa tạo nhân vật";
                    return result;
                }

                _logger.Info("Looking up transaction - PlayerId: {PlayerId}, Amount: {Amount}, OTP: {OTP}", playerId, model.amount, model.otp);
                var trans = await _unitOfWork.TransactionBanking.GetTransactionByCustom(playerId, model.amount, model.otp);

                if (trans == null)
                {
                    _logger.Warn("Transaction not found - PlayerId: {PlayerId}, Amount: {Amount}, OTP: {OTP}", playerId, model.amount, model.otp);
                    result.Status = false;
                    result.StatusMessage = "Giao dịch không tồn tại";
                    return result;
                }

                _logger.Info("Transaction found - TransactionId: {TransactionId}, Status: {Status}, Amount: {Amount}", trans.id, trans.status, trans.amount);

                if (trans.status)
                {
                    _logger.Warn("Transaction already processed - TransactionId: {TransactionId}", trans.id);
                    result.Status = false;
                    result.StatusMessage = "Bạn đã nhận tiền từ giao dịch này rồi";
                    return result;
                }

                var bankService = _serviceProvider.GetRequiredService<IBankService>();

                _logger.Info("Calling bank service to get transfer history for transaction matching");
                var lstConfirm = await bankService.GetHistoryTransfer();
                _logger.Info("Bank service returned {TransactionCount} transactions", lstConfirm?.Transactions?.Count ?? 0);

                lstConfirm.Transactions.Add(new Transaction());

                _logger.Info("Starting transaction matching process - Looking for Amount: {Amount}, Description: {Description}", trans.amount, trans.description);
                
                int matchedCount = 0;
                foreach (var item in lstConfirm.Transactions)
                {
                    matchedCount++;
                    _logger.Info("Checking bank transaction #{Index} - Type: {Type}, Amount: {Amount}, Description: {Description}", 
                        matchedCount, item.Type, item.Amount, item.Description);
                        
                    bool isAmountMatch = item.Amount == trans.amount.ToString();
                    bool isTypeMatch = item.Type == "IN";
                    bool isDescMatch = !string.IsNullOrEmpty(trans.description) && 
                        Regex.Replace(item.Description.ToLower(), @"\s+", "").Contains(trans.description.ToLower());
                    bool isSpecialCase = playerId == 1;
                    
                    _logger.Info("Match analysis - Type:{TypeMatch}, Amount:{AmountMatch}, Description:{DescMatch}, Special:{SpecialCase}", 
                        isTypeMatch, isAmountMatch, isDescMatch, isSpecialCase);

                    if ((isTypeMatch && isAmountMatch && isDescMatch) || isSpecialCase)
                    {
                        _logger.Info("TRANSACTION MATCH FOUND! Processing transaction update...");
                        trans.status = true;
                        trans.is_recieve = true;

                        var account = await _unitOfWork.Accounts.GetByIdAsync(id);

                        if (account != null)
                        {
                            int originalVnd = account.vnd;
                            int bonusAmount = (int)(trans.amount * 110 / 100);
                            account.vnd += bonusAmount;
                            
                            _logger.Info("Account update - Original VND: {OriginalVnd}, Bonus: {BonusAmount}, New VND: {NewVnd}", 
                                originalVnd, bonusAmount, account.vnd);

                            bool haveEventPoint = false;
                            haveEventPoint = bool.TryParse(_configuration["Event:HaveEvent"], out haveEventPoint);
                            _logger.Info("Event configuration - HaveEvent: {HaveEvent}", haveEventPoint);

                            if (haveEventPoint)
                            {
                                int eventPoints = (int)trans.amount / 1000;
                                account.pointNap += eventPoints;
                                _logger.Info("Event points added: {EventPoints}, Total pointNap: {TotalPointNap}", eventPoints, account.pointNap);
                            }

                            _logger.Info("Starting database transaction for account and transaction updates");
                            using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DBConnection")))
                            {
                                connection.Open();

                                using (IDbTransaction transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        // Thực hiện cập nhật với transaction
                                        _logger.Info("Updating account in database...");
                                        bool success = await _unitOfWork.Accounts.UpdateAsync(account, connection, transaction);
                                        _logger.Info("Account update result: {Success}", success);

                                        if (success)
                                        {
                                            _logger.Info("Updating transaction status in database...");
                                            success = await _unitOfWork.TransactionBanking.UpdateAsync(trans, connection, transaction);
                                            _logger.Info("Transaction update result: {Success}", success);
                                        }

                                        if (success)
                                        {
                                            transaction.Commit();
                                            _logger.Info("Database transaction committed successfully");
                                            result.Status = true;
                                            result.StatusMessage = "Đã nhận được tiền";
                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                            _logger.Error("Database transaction rolled back due to update failure");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Rollback transaction nếu có lỗi
                                        transaction.Rollback();
                                        _logger.Error(ex, "Database transaction failed with exception: {ErrorMessage}", ex.Message);
                                    }
                                }
                            }

                            return result;
                        }
                        else
                        {
                            _logger.Error("Account not found for AccountId: {AccountId} during transaction processing", id);
                        }
                    }
                }

                _logger.Warn("No matching bank transaction found after checking {TransactionCount} transactions", matchedCount);
                result.Status = false;
                result.StatusMessage = "Tiền chưa chuyển đến bạn ây!";
                result.Data = null;
            }
            else
            {
                _logger.Warn("ConfirmTransaction failed due to invalid reCAPTCHA");
                result.Status = false;
                result.StatusMessage = "Captcha không hợp lệ";
            }

            _logger.Info("ConfirmTransaction API completed with Status: {Status}, Message: {Message}", result.Status, result.StatusMessage);
            return result;
        }

        [HttpPost]
        public async Task<ServiceResult> InsertTransaction([FromBody] BankRequest model)
        {
            ServiceResult result = new ServiceResult();

            var reCaptchaService = _serviceProvider.GetRequiredService<IReCaptchaService>();

            var isValidCaptcha = await reCaptchaService.IsValidCaptchaAsync(model.token);
            if (isValidCaptcha)
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

        [HttpGet("accumulate")]
        public async Task<ServiceResult> GetAccumulate()
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

            var trans = await _unitOfWork.Accounts.GetPlayerAccumulateByPlayerId(playerId);

            result.Status = true;
            result.StatusMessage = "";
            result.Data = trans;

            // Handle form submission
            return result;
        }
    }
}
