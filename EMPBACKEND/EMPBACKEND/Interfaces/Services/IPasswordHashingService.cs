namespace EMPBACKEND.Interfaces.Services
{
    public interface IPasswordHashingService
    {
        string GenerateSalt();
        string HashPassword(string password, string salt);
        bool VerifyPassword(string password, string salt, string hashedPassword);
    }
}