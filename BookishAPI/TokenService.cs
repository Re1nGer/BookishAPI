using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
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
}
