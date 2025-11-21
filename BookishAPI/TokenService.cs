using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BookishAPI;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private const int RefreshTokenValidityInDays = 7;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public TokenResponse GenerateTokens(Guid userId)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JWT:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        
        var refresh = new JwtSecurityToken(
              issuer: _configuration["JWT:ValidIssuer"],
              claims: claims,
              expires: DateTime.UtcNow.AddDays(RefreshTokenValidityInDays),
              signingCredentials: credentials
        ); 

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(refresh);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    public TokenResponse RotateTokens(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
    
        try
        {
            // Validate the refresh token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Remove default 5 min clock skew
            };

            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);
        
            // Verify it's a JWT token with correct algorithm
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Extract userId from claims
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new SecurityTokenException("Invalid user ID in token");
            }

            // Generate new token pair
            return GenerateTokens(userId);
        }
        catch (SecurityTokenExpiredException)
        {
            throw new SecurityTokenException("Refresh token has expired");
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Invalid refresh token", ex);
        }
    }
}
