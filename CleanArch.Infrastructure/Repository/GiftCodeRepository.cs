using CleanArch.Application.Interfaces;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CleanArch.Infrastructure.Repository
{
    public class GiftCodeRepository : BaseRepository<GiftCode>, IGiftCodeRepository
    {
        public GiftCodeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<string> GenerateUniqueGiftCodeAsync()
        {
            string code;
            bool exists = true;
            var random = new Random();
            
            do
            {
                // Tạo mã gift code ngẫu nhiên 10 ký tự
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                code = new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                
                exists = await IsGiftCodeExistsAsync(code);
            } 
            while (exists);
            
            return code;
        }

        public async Task<bool> IsGiftCodeExistsAsync(string code)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    const string query = "SELECT COUNT(1) FROM gift_codes WHERE code = @Code";
                    var count = await connection.QuerySingleAsync<int>(query, new { Code = code });
                    return count > 0;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return true; // Để an toàn, coi như đã tồn tại
                }
            }
        }

        public async Task<long> AddAndReturnIdAsync(GiftCode giftCode)
        {
            using (IDbConnection connection = new MySqlConnection(configuration.GetConnectionString("DBConnection")))
            {
                try
                {
                    // Bước 1: Insert gift code
                    const string insertQuery = @"
                        INSERT INTO gift_codes (type, code, gold, gem, ruby, items, status, active, expires_at, created_at, updated_at)
                        VALUES (@Type, @Code, @Gold, @Gem, @Ruby, @Items, @Status, @Active, @ExpiresAt, @CreatedAt, @UpdatedAt)";

                    await connection.ExecuteAsync(insertQuery, giftCode);

                    // Bước 2: Select ID theo code (unique) để tránh race condition
                    const string selectQuery = "SELECT id FROM gift_codes WHERE code = @Code";
                    var id = await connection.QuerySingleAsync<long>(selectQuery, new { Code = giftCode.Code });
                    
                    return id;
                }
                catch (Exception ex)
                {
                    // Log exception
                    return 0;
                }
            }
        }
    }
}
