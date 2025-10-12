using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Espectaculos.WebApi.Options;
using Microsoft.Extensions.Options;

namespace Espectaculos.WebApi.Security;

public class HmacValidationTokenService : IValidationTokenService
{
    private readonly ValidationTokenOptions _options;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // Skew permitido ±2 minutos
    private static readonly TimeSpan Skew = TimeSpan.FromMinutes(2);

    public HmacValidationTokenService(IOptions<ValidationTokenOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(Guid orderId, TimeSpan? ttl = null)
    {
        var secret = _options.Secret;
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("ValidationTokens:Secret no configurado.");

        var now = DateTimeOffset.UtcNow;
        var exp = now.Add(ttl ?? TimeSpan.FromMinutes(_options.DefaultExpiryMinutes)).ToUnixTimeSeconds();

        var payloadObj = new { orderId, exp, nonce = Guid.NewGuid() };
        var payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions);
        var payloadSegment = ToBase64Url(Encoding.UTF8.GetBytes(payloadJson));

        var signatureBytes = ComputeHmacSha256(Encoding.UTF8.GetBytes(payloadSegment), Encoding.UTF8.GetBytes(secret));
        var signatureSegment = ToBase64Url(signatureBytes);

        return $"{payloadSegment}.{signatureSegment}";
    }

    public TokenCheckResult Validate(string token)
    {
        var secret = _options.Secret;
        if (string.IsNullOrWhiteSpace(secret))
            return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Secret no configurado");

        if (string.IsNullOrWhiteSpace(token))
            return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Token vacío");

        var parts = token.Split('.');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Formato inválido");

        var payloadSegment = parts[0];
        var signatureSegment = parts[1];

        byte[] providedSignature;
        try { providedSignature = FromBase64Url(signatureSegment); }
        catch { return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Firma inválida"); }

        var expectedSignature = ComputeHmacSha256(Encoding.UTF8.GetBytes(payloadSegment), Encoding.UTF8.GetBytes(secret));
        if (!CryptographicOperations.FixedTimeEquals(expectedSignature, providedSignature))
            return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Firma no válida");

        try
        {
            var payloadBytes = FromBase64Url(payloadSegment);
            using var doc = JsonDocument.Parse(payloadBytes);
            var root = doc.RootElement;

            if (!root.TryGetProperty("orderId", out var orderIdProp) ||
                !root.TryGetProperty("exp", out var expProp))
                return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Payload incompleto");

            if (!Guid.TryParse(orderIdProp.GetString(), out var orderId))
                return new TokenCheckResult(TokenCheckStatus.Invalid, null, "orderId inválido");

            var nowEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = expProp.GetInt64();

            if (exp + (long)Skew.TotalSeconds <= nowEpoch)
                return new TokenCheckResult(TokenCheckStatus.Expired, orderId, "Token expirado");

            return new TokenCheckResult(TokenCheckStatus.Ok, orderId, null);
        }
        catch
        {
            return new TokenCheckResult(TokenCheckStatus.Invalid, null, "Payload inválido");
        }
    }

    private static byte[] ComputeHmacSha256(byte[] data, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }

    private static string ToBase64Url(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] FromBase64Url(string base64Url)
    {
        string padded = base64Url.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4) { case 2: padded += "=="; break; case 3: padded += "="; break; }
        return Convert.FromBase64String(padded);
    }
}
