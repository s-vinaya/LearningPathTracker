using EMPBACKEND.Models;
using System.Security.Claims;

namespace EMPBACKEND.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}