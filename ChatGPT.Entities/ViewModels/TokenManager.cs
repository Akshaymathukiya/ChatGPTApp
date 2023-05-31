using ChatGPT.Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Entities.ViewModels
{
    public class TokenManager
    {
        
        public string GenerateToken(TokenKeyViewModel jwt, User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwt.Key);
            var claims = new ClaimsIdentity(new Claim[]
              {
                new Claim(ClaimTypes.Name,user.Id.ToString()),
                new Claim(ClaimTypes.UserData,user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Firstname + " "+ user.Lastname),
                //new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRole),value: user.Role)),

            });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                IssuedAt = DateTime.UtcNow,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
