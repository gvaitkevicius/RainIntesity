using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secretKey;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtService(string secretKey)
    {
        _secretKey = secretKey;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
    }

    public string GenerateToken(string username)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = _signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}