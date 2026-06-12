using System.Security.Cryptography;
using System.Text;

namespace Heralabs.ClientManagement.Core.Helpers
{
    public static class HmacHelper
    {
        /// <summary>
        /// Sinh ra một chuỗi Secret Key ngẫu nhiên và bảo mật cao.
        /// Mặc định dùng 32 bytes (256 bits) - Rất phù hợp và an toàn cho thuật toán HMAC-SHA256.
        /// </summary>
        /// <param name="lengthInBytes">Độ dài của mảng byte (mặc định 32)</param>
        /// <returns>Chuỗi Secret Key định dạng Base64</returns>
        public static string GenerateSecretKey(int lengthInBytes = 32)
        {
            byte[] keyBytes = new byte[lengthInBytes];

            // Sử dụng RandomNumberGenerator để đảm bảo tính ngẫu nhiên an toàn (chuẩn mật mã học)
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            // Trả về chuỗi Base64. 
            // Nếu bạn không thích các ký tự đặc biệt như + / = trong Base64, 
            // bạn có thể đổi thành Convert.ToHexString(keyBytes) để lấy chuỗi Hex (A-F, 0-9).
            return Convert.ToBase64String(keyBytes);
        }

        /// <summary>
        /// Tính toán chữ ký HMAC-SHA256 dựa trên payload và secret key.
        /// </summary>
        /// <returns>Chữ ký định dạng Base64</returns>
        public static string GenerateSignature(string method, string path, long timestamp, string body, string secretKey)
        {
            // Gộp chuỗi theo đúng format đã thống nhất ở Filter và Postman
            string payload = $"{method.ToUpper()}{path}{timestamp}{body}";

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(payloadBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
