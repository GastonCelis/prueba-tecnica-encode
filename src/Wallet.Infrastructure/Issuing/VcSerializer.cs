using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wallet.Domain.Contracts;
using Wallet.Domain.Enums;

namespace Wallet.Infrastructure.Issuing;

/// <summary>
/// Serializa la credencial completa (incluido proof) para su persistencia.
/// A diferencia del formato canónico, respeta el orden de la tabla del
/// enunciado y no se usa para calcular la firma.
/// </summary>
public static class VcSerializer
{
    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter<Categoria>() }
    };

    public static string Serializar(VerifiableCredential vc) =>
        JsonSerializer.Serialize(new
        {
            id = vc.Id,
            type = vc.Type,
            issuer = vc.Issuer,
            credentialSubject = new
            {
                id = vc.CredentialSubject.Id,
                nombre = vc.CredentialSubject.Nombre,
                apellido = vc.CredentialSubject.Apellido,
                dni = vc.CredentialSubject.Dni,
                numeroSocio = vc.CredentialSubject.NumeroSocio,
                categoria = vc.CredentialSubject.Categoria,
                foto = vc.CredentialSubject.Foto
            },
            validFrom = CredentialCanonicalizer.FormatearUtc(vc.ValidFrom),
            validUntil = CredentialCanonicalizer.FormatearUtc(vc.ValidUntil),
            credentialStatus = (int)vc.CredentialStatus,
            proof = new
            {
                type = vc.Proof.Type,
                created = CredentialCanonicalizer.FormatearUtc(vc.Proof.Created),
                verificationMethod = vc.Proof.VerificationMethod,
                proofValue = vc.Proof.ProofValue
            }
        }, Opciones);
}