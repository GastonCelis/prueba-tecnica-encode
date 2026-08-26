namespace Wallet.Infrastructure.Issuing;

public sealed class IssuerOptions
{
    public const string Seccion = "Issuer";

    public string Did { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string VerificationMethod { get; set; } = string.Empty;
}