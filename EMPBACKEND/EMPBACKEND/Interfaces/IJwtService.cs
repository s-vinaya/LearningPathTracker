using EMPBACKEND.Models;

namespace EMPBACKEND.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}