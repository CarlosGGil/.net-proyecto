namespace Espectaculos.WebApi.Options;

public class ValidationTokenOptions
{
    public string Secret { get; set; } = string.Empty;
    public int DefaultExpiryMinutes { get; set; } = 1440;
}
