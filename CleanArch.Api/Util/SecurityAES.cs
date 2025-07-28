using System.Security.Cryptography;
using System.Text;

namespace CleanArch.Api.Util
{
    public static class SecurityAES
    {
        // Kích thước của các thành phần trong gói dữ liệu mã hóa
        private const int SaltSize = 16; // Salt để tăng cường bảo mật cho mật khẩu
        private const int NonceSize = 12; // Nonce (Number used once) cho AES-GCM
        private const int TagSize = 16;   // Tag xác thực cho AES-GCM

        // Số vòng lặp cho PBKDF2. Càng cao càng an toàn nhưng càng chậm.
        // Con số này nên được cập nhật theo khuyến nghị bảo mật hiện hành.
        private const int Pbkdf2Iterations = 100000;

        /// <summary>
        /// Dẫn xuất một khóa mã hóa 256-bit (32-byte) từ mật khẩu và salt.
        /// </summary>
        /// <param name="password">Mật khẩu người dùng cung cấp.</param>
        /// <param name="salt">Một chuỗi byte ngẫu nhiên (salt).</param>
        /// <returns>Một khóa mã hóa 32-byte.</returns>
        private static byte[] DeriveKeyFromPassword(string password, byte[] salt)
        {
            // Sử dụng PBKDF2 để "kéo dãn" mật khẩu thành một khóa có độ dài phù hợp
            // và đủ độ phức tạp về mặt mật mã học.
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256))
            {
                return deriveBytes.GetBytes(32); // Lấy 32 byte để làm khóa AES-256
            }
        }

        /// <summary>
        /// Mã hóa một chuỗi văn bản bằng mật khẩu.
        /// </summary>
        /// <param name="plainText">Chuỗi văn bản cần mã hóa.</param>
        /// <param name="password">Mật khẩu dùng để tạo khóa mã hóa.</param>
        /// <returns>
        /// Một chuỗi Base64 chứa dữ liệu đã mã hóa.
        /// Cấu trúc: [Salt (16 bytes)][Nonce (12 bytes)][Tag (16 bytes)][Ciphertext (n bytes)]
        /// </returns>
        public static string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // 1. Tạo một salt ngẫu nhiên cho mỗi lần mã hóa.
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // 2. Dẫn xuất khóa mã hóa từ mật khẩu và salt.
            byte[] key = DeriveKeyFromPassword(password, salt);

            // 3. Chuyển văn bản gốc thành byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // 4. Chuẩn bị các mảng byte cho nonce, tag, và ciphertext.
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[plainTextBytes.Length];

            // 5. Thực hiện mã hóa.
            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Encrypt(nonce, plainTextBytes, ciphertext, tag);
            }

            // 6. Ghép nối Salt + Nonce + Tag + Ciphertext để tạo thành kết quả cuối cùng.
            byte[] encryptedData = new byte[SaltSize + NonceSize + TagSize + ciphertext.Length];
            Buffer.BlockCopy(salt, 0, encryptedData, 0, SaltSize);
            Buffer.BlockCopy(nonce, 0, encryptedData, SaltSize, NonceSize);
            Buffer.BlockCopy(tag, 0, encryptedData, SaltSize + NonceSize, TagSize);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, SaltSize + NonceSize + TagSize, ciphertext.Length);

            // 7. Trả về dưới dạng chuỗi Base64.
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Giải mã một chuỗi đã được mã hóa bằng mật khẩu.
        /// </summary>
        /// <param name="encryptedText">Chuỗi Base64 chứa dữ liệu mã hóa.</param>
        /// <param name="password">Mật khẩu đã dùng để mã hóa.</param>
        /// <returns>Chuỗi văn bản gốc đã được giải mã.</returns>
        public static string Decrypt(string encryptedText, string password)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException(nameof(encryptedText));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // 1. Chuyển chuỗi Base64 về lại byte array.
            byte[] encryptedData = Convert.FromBase64String(encryptedText);

            if (encryptedData.Length < SaltSize + NonceSize + TagSize)
                throw new ArgumentException("Dữ liệu mã hóa không hợp lệ.", nameof(encryptedText));

            // 2. Tách Salt ra từ đầu dữ liệu.
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(encryptedData, 0, salt, 0, SaltSize);

            // 3. Dẫn xuất lại khóa mã hóa từ mật khẩu và salt đã tách ra.
            // Bước này cực kỳ quan trọng, phải dùng đúng salt thì mới ra đúng khóa.
            byte[] key = DeriveKeyFromPassword(password, salt);

            // 4. Tách Nonce, Tag, và Ciphertext.
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[encryptedData.Length - SaltSize - NonceSize - TagSize];

            Buffer.BlockCopy(encryptedData, SaltSize, nonce, 0, NonceSize);
            Buffer.BlockCopy(encryptedData, SaltSize + NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(encryptedData, SaltSize + NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

            // 5. Thực hiện giải mã.
            byte[] decryptedBytes = new byte[ciphertext.Length];
            using (var aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
            }

            // 6. Chuyển mảng byte đã giải mã về lại chuỗi String.
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
