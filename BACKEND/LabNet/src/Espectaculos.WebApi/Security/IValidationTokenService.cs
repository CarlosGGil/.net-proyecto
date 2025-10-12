using System;

namespace Espectaculos.WebApi.Security;

public interface IValidationTokenService
{
    string Generate(Guid orderId, TimeSpan? ttl = null);
    TokenCheckResult Validate(string token);
}

public enum TokenCheckStatus { Ok, Invalid, Expired }

public sealed record TokenCheckResult(TokenCheckStatus Status, Guid? OrderId, string? Detail);
