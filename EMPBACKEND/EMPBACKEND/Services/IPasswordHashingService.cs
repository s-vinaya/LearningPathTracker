using System.Security.Cryptography;
using System.Text;

namespace EMPBACKEND.Services
{
    public interface IPasswordHashingService
    {
        string GenerateSalt();
        string HashPassword(string password, string salt);
        bool VerifyPassword(string password, string salt, string hashedPassword);
    }

    public class PasswordHashingService : IPasswordHashingService
    {
        public string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
            
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);
            
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            var computedHash = HashPassword(password, salt);
            return computedHash == hashedPassword;
        }
    }
}