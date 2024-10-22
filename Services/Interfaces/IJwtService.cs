using System.Security.Claims;
using G3NexusBackend.Models;

namespace G3NexusBackend.Interfaces
{
public interface IJwtService
{
    string GenerateAccessToken(string userName, string[] roles);
    string GenerateRefreshToken();
    Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<bool> DeleteRefreshTokenAsync(string token);
    Task<RefreshToken?> GetRefreshTokenData(string token);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    
}
    
}

