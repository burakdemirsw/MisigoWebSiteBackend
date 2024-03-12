using GooleAPI.Application.Abstractions.IServices.IHelper;
using System.Security.Cryptography;
using System.Text;

namespace GoogleAPI.Persistance.Concreates.Services.Helper
{
    public class HelperService : IHelperService
    {
        public string ComputeHMACSHA256(string data, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyHMACSHA256(string originalData, string hashedData, string key)
        {
            string computedHash = ComputeHMACSHA256(originalData, key);
            return string.Equals(computedHash, hashedData, StringComparison.OrdinalIgnoreCase);
        }
    }
}
