using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models.User;
using GooleAPI.Application.Abstractions;
using GooleAPI.Application.IRepositories.UserAndCommunication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace GoogleAPI.Persistance.Concreates
{
    public class TokenService : ITokenService
    {
        readonly IConfiguration _configuration;
        readonly IUserReadRepository _ur;


        public TokenService(IConfiguration configuration, IUserReadRepository userReadRepository)
        {
            _configuration = configuration;
            _ur = userReadRepository;

        }

        public async Task<Token> CreateAccsessToken(int minute, User user)
        {
            Token token = new Token();

            SymmetricSecurityKey securityKey =
                new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.Now.AddMinutes(minute);
            JwtSecurityToken securityToken =
                new(
                    audience: _configuration["Token:Audience"],
                    issuer: _configuration["Token:Issuer"],
                    expires: token.Expiration,
                    notBefore: DateTime.UtcNow,
                    signingCredentials: signingCredentials,
                    claims: new List<Claim> { new(ClaimTypes.Name, user.Email) }
                );
            token.RefreshToken = await CreateRefreshToken();
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            return token;
        }
        public async Task<Token> CreatePasswordResetToken(int minute, User user)
        {
            Token token = new Token();

            SymmetricSecurityKey securityKey =
                new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            token.Expiration = DateTime.Now.AddMinutes(minute);
            JwtSecurityToken securityToken =
                new(
                    audience: _configuration["Token:Audience"],
                    issuer: _configuration["Token:Issuer"],
                    expires: token.Expiration,
                    notBefore: DateTime.UtcNow,
                    signingCredentials: signingCredentials,
                    claims: new List<Claim> { new(ClaimTypes.Name, user.Email) }
                );
            token.RefreshToken = null;
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            return token;
        }



        public async Task<string> CreateRefreshToken( )
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }


        public async Task<string> CreatePasswordChangeToken( )
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }


    }
}
