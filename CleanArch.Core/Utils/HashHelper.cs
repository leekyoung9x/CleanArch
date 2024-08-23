using System.Security.Cryptography;
using System.Text;

namespace CleanArch.Core.Utils
{
    public static class HashHelper
    {
        public static string ComputeMd5Hash(string input)
        {
            // Tạo một đối tượng MD5
            using (MD5 md5 = MD5.Create())
            {
                // Chuyển đổi chuỗi đầu vào thành một mảng byte
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                
                // Tính toán giá trị băm MD5
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                
                // Chuyển đổi mảng byte thành chuỗi hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                // Trả về chuỗi đã mã hóa
                return sb.ToString();
            }
        }
    }
}


