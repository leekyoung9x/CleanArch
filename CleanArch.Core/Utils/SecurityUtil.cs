using System.Security.Cryptography;
using System.Text;

namespace CleanArch.Core.Utils
{
    public class SecurityUtil
    {
        public static string GenerateOtp(int length = 6)
        {
            // Tạo một chuỗi thời gian hiện tại với độ chính xác cao
            string timestamp = DateTime.UtcNow.Ticks.ToString();

            // Tạo một chuỗi ngẫu nhiên từ bộ tạo số ngẫu nhiên an toàn
            string randomString = GenerateRandomString(length);

            // Kết hợp timestamp và chuỗi ngẫu nhiên
            string combinedString = timestamp + randomString;

            // Sử dụng hàm băm SHA256 để tạo OTP từ chuỗi kết hợp
            string otpHash = ComputeSha256Hash(combinedString);

            // Lấy số ký tự mong muốn từ OTP hash
            return otpHash.Substring(0, length);
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            var stringBuilder = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                stringBuilder.Append(chars[b % chars.Length]);
            }

            return stringBuilder.ToString();
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Chuyển đổi chuỗi thành mảng byte
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Chuyển đổi mảng byte thành chuỗi hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}