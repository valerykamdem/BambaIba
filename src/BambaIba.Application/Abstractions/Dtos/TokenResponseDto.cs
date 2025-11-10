using System.IdentityModel.Tokens.Jwt;

namespace BambaIba.Application.Abstractions.Dtos;

public class TokenResponseDto
{
    public string Access_Token { get; set; } = string.Empty;
    public string Refresh_Token { get; set; } = string.Empty;
    public int Expires_In { get; set; }
    public string UserId => ExtractUserIdFromToken(Access_Token);

    private string ExtractUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Subject;
    }
}
