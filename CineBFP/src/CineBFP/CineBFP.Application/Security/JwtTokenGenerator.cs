using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CineBFP.Application.Security
{
    public class JwtTokenGenerator
    {
        private readonly JwtOptions _opt;
        public JwtTokenGenerator(IOptions<JwtOptions> opt) => _opt = opt.Value;

        public string Create(string name, string email, string role, string usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("name",    name),
                new Claim(ClaimTypes.Role, role),
                new Claim("usuario", usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_opt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opt.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
