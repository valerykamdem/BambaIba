using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace BambaIba.Application.Abstractions.Dtos;

public record CursorData(DateTime CreatedAt, Guid Id);

public static class CursorExtensions
{
    // 2. Méthode Générique d'Encodage
    public static string Encode<T>(T cursor) where T : CursorData
    {
        string json = JsonSerializer.Serialize(cursor);
        return Base64UrlEncoder.Encode(json);
    }

    // 3. Méthode Générique de Décodage
    public static T? Decode<T>(string? cursor) where T : CursorData
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;
        try
        {
            string json = Base64UrlEncoder.Decode(cursor);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return null;
        }
    }
}
