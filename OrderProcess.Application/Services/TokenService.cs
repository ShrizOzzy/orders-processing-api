using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderProcess.Core.Models.LoginDto;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace OrderProcess.Application.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public LoginResponseDto GenerateToken(string username)
        {
            var jwt = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                Expiration = token.ValidTo
            };
        }
    }
}
