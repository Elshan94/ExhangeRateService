using BambooExhangeRateService.Application.Models;
using BambooExhangeRateService.Application.Services.Abstract;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BambooExhangeRateService.Application.Services.Concrete
{
    public class JwtTokenService : ITokenService
    {
        public JwtConfig JwtConfig { get; }
        public JwtTokenService(JwtConfig jwtConfig)
        {
            JwtConfig = jwtConfig;
        }

        public string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(JwtConfig.Issuer, JwtConfig.Audience, claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
