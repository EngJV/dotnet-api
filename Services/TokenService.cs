using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        // What is a SymmetricSecurityKey?. A SymmetricSecurityKey is a type of cryptographic key that is used in symmetric encryption algorithms. It is a symmetric key because it is used for both encryption and decryption of data.
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // The WALLET is the object that contains the information about the token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // The subject is the entity that is responsible for generating the token.
                Expires = DateTime.Now.AddDays(7), // The expiration date is the date and time when the token will expire.
                SigningCredentials = creds, // The signing credentials are the credentials that are used to sign the token.
                Issuer = _config["JWT:Issuer"], // The issuer is the entity that is responsible for generating the token.
                Audience = _config["JWT:Audience"] // The audience is the entity that is responsible for validating the token.
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // The token handler is the object that is responsible for creating the token.

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}