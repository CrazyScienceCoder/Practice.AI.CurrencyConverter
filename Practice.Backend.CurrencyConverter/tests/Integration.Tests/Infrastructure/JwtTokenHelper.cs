using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

public static class JwtTokenHelper
{
    public static string GenerateToken(params string[] roles)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(CustomWebApplicationFactory.TestSigningKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("sub", "test-user-id"),
            new("name", "Test User")
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
