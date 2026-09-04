using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth2.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
namespace auth2.Services;

public class JwtService
{
    private readonly IConfiguration _configutration;
    private readonly SymmetricSecurityKey _key;

    public JwtService(IConfiguration configutayion)
    {
        _configutration = configutayion;
        var secretKey = _configutration["JwtSettings:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
    }
    public string GenerateToken(User user)
    {
        var claim  = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name, (user.Name ?? "Unknown")),
            new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString())
        };

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claim),
            Audience= _configutration["JwtSettings:Audience"],
            Issuer = _configutration["JwtSettings:Issuer"],
            SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha256),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configutration["JwtSettings:ExpirationInMinutes"]))   
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }
}