using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Espectaculos.WebApi.Utils;

public static class ValidationTokenHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string CreateToken(Guid orderId, string secret, TimeSpan ttl)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret requerido para generar token.", nameof(secret));

        var now = DateTimeOffset.UtcNow;
        var exp = now.Add(ttl).ToUnixTimeSeconds();

        var payloadObj = new
        {
            orderId,
            exp,
            nonce = Guid.NewGuid()
        };

        var payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions);
        var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
        var payloadSegment = ToBase64Url(payloadBytes);

        var signatureBytes = ComputeHmacSha256(Encoding.UTF8.GetBytes(payloadSegment), Encoding.UTF8.GetBytes(secret));
        var signatureSegment = ToBase64Url(signatureBytes);

        return $"{payloadSegment}.{signatureSegment}";
    }

    public static TokenValidationResult ValidateToken(string token, string? secret)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Token vacío");

        if (string.IsNullOrWhiteSpace(secret))
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Secret no configurado");

        var parts = token.Split('.');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Formato inválido");

        var payloadSegment = parts[0];
        var signatureSegment = parts[1];

        byte[] providedSignature;
        try
        {
            providedSignature = FromBase64Url(signatureSegment);
        }
        catch
        {
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Firma inválida");
        }

        var expectedSignature = ComputeHmacSha256(Encoding.UTF8.GetBytes(payloadSegment), Encoding.UTF8.GetBytes(secret));
        if (!CryptographicOperations.FixedTimeEquals(expectedSignature, providedSignature))
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Firma no válida");

        try
        {
            var payloadBytes = FromBase64Url(payloadSegment);
            using var doc = JsonDocument.Parse(payloadBytes);
            var root = doc.RootElement;

            if (!root.TryGetProperty("orderId", out var orderIdProp) ||
                !root.TryGetProperty("exp", out var expProp) ||
                !root.TryGetProperty("nonce", out var nonceProp))
            {
                return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Payload incompleto");
            }

            if (!Guid.TryParse(orderIdProp.GetString(), out var orderId))
                return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "orderId inválido");

            // Validar nonce como GUID (no se usa, solo antireplay-construction)
            _ = Guid.TryParse(nonceProp.GetString(), out _);

            var nowEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = expProp.GetInt64();
            if (exp <= nowEpoch)
                return new TokenValidationResult(TokenValidationStatus.Expired, orderId, "Token expirado");

            return new TokenValidationResult(TokenValidationStatus.Ok, orderId, null);
        }
        catch
        {
            return new TokenValidationResult(TokenValidationStatus.InvalidToken, null, "Payload inválido");
        }
    }

    private static byte[] ComputeHmacSha256(byte[] data, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] FromBase64Url(string base64Url)
    {
        string padded = base64Url.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String(padded);
    }
}

public enum TokenValidationStatus
{
    Ok,
    InvalidToken,
    Expired
}

public record TokenValidationResult(TokenValidationStatus Status, Guid? OrderId, string? Detail);
