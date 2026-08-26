using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Wallet.Domain.Contracts;
using Wallet.Domain.Enums;

namespace Wallet.Infrastructure.Issuing;

public sealed class HmacCredentialIssuer : ICredentialIssuer
{
    private const string TipoDeFirma = "HMAC-SHA256";
    private static readonly string[] TipoDeCredencial =
        ["VerifiableCredential", "SocioCredential"];

    private readonly IssuerOptions _opciones;

    public HmacCredentialIssuer(IOptions<IssuerOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public VerifiableCredential Emitir(CredentialSubject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        if (string.IsNullOrWhiteSpace(_opciones.SigningKey))
            throw new InvalidOperationException(
                "No hay clave de firma configurada.");

        var rawId = Guid.NewGuid();
        var id = $"{_opciones.BaseUrl.TrimEnd('/')}/{rawId}";

        var validFrom = CredentialCanonicalizer.TruncarASegundos(DateTimeOffset.UtcNow);
        var validUntil = validFrom.AddYears(1);
        const CredentialStatus status = CredentialStatus.Active;

        var canonico = CredentialCanonicalizer.Canonicalizar(
            id, _opciones.Did, TipoDeCredencial, subject, validFrom, validUntil, status);

        var proofValue = Firmar(canonico, _opciones.SigningKey);

        var proof = new Proof(
            Type: TipoDeFirma,
            Created: validFrom,
            VerificationMethod: _opciones.VerificationMethod,
            ProofValue: proofValue);

        return new VerifiableCredential(
            rawId, id, TipoDeCredencial, _opciones.Did,
            subject, validFrom, validUntil, status, proof);
    }

    private static string Firmar(string canonico, string clave)
    {
        var bytesClave = Encoding.UTF8.GetBytes(clave);
        var bytesMensaje = Encoding.UTF8.GetBytes(canonico);
        var hash = HMACSHA256.HashData(bytesClave, bytesMensaje);
        return Convert.ToBase64String(hash);
    }

    public string Serializar(VerifiableCredential vc) => VcSerializer.Serializar(vc);
}