using MessageNowApi.Data;
using MessageNowApi.Dtos;
using MessageNowApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace MessageNowApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<MessageNowUser> _userManager;
        private readonly IdentityContext _identityContext;
        public TokenService(IOptions<JwtOptions> jwtOptions, UserManager<MessageNowUser> userManager, IdentityContext identityContext) 
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _identityContext = identityContext;
        }

        public async Task<string> CreateToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key)), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        public async Task<string?> CreateTokenByRefreshToken(string refreshToken)
        {
            var refreshTokenModel = await _identityContext.RefreshTokens.Where(x => x.Token ==  refreshToken).AsNoTracking().FirstOrDefaultAsync();
            if(refreshTokenModel == null || !refreshTokenModel.Active)
            {
                return null;
            }
            var token = await CreateToken(refreshTokenModel.Username);
            return token;
        }

        public async Task<string> CreateRefreshToken(string username)
        {
            var token = await CreateToken(username);
            var refreshToken = new RefreshToken()
            {
                Token = token,
                Username = username,
                Active = true,
            };
            await _identityContext.RefreshTokens.AddAsync(refreshToken);
            await _identityContext.SaveChangesAsync();
            return token;
        }

        public Guid? GetUserIdByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            TokenValidationParameters tvp = new TokenValidationParameters();
            tvp.ValidateIssuer = false;
            tvp.ValidateAudience = false;
            tvp.ValidateIssuerSigningKey = true;
            tvp.ValidateLifetime = false;
            tvp.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            SecurityToken secureToken;
            var principal = handler.ValidateToken(token, tvp, out secureToken);
            if(principal == null)
            {
                return null;
            }
            var idString = principal.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if(idString == null)
            {
                return null;
            }
            return new Guid(idString);
        }

        public async Task DeactiveRefreshToken(string token)
        {
            var refreshToken = await _identityContext.RefreshTokens.Where(x => x.Token == token).FirstOrDefaultAsync();
            if (refreshToken == null)
            {
                return;
            }
            refreshToken.Active = false;
            await _identityContext.RefreshTokens.AddAsync(refreshToken);
            await _identityContext.SaveChangesAsync();
        }
    }
}
