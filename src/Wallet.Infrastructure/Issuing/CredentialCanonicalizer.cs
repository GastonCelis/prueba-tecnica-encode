using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wallet.Domain.Contracts;
using Wallet.Domain.Enums;

namespace Wallet.Infrastructure.Issuing;

public static class CredentialCanonicalizer
{
    private const string FormatoFecha = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter<Categoria>() }
    };

    public static string Canonicalizar(
        string id,
        string issuer,
        IReadOnlyList<string> type,
        CredentialSubject subject,
        DateTimeOffset validFrom,
        DateTimeOffset validUntil,
        CredentialStatus status)
    {
        var canonica = new CanonicalCredential
        {
            CredentialStatus = (int)status,
            CredentialSubject = new CanonicalSubject
            {
                Apellido = subject.Apellido,
                Categoria = subject.Categoria,
                Dni = subject.Dni,
                Foto = subject.Foto,
                Id = subject.Id,
                Nombre = subject.Nombre,
                NumeroSocio = subject.NumeroSocio
            },
            Id = id,
            Issuer = issuer,
            Type = type,
            ValidFrom = FormatearUtc(validFrom),
            ValidUntil = FormatearUtc(validUntil)
        };

        return JsonSerializer.Serialize(canonica, Opciones);
    }

    public static string FormatearUtc(DateTimeOffset valor) =>
        valor.ToUniversalTime().ToString(FormatoFecha, CultureInfo.InvariantCulture);

    public static DateTimeOffset TruncarASegundos(DateTimeOffset valor) =>
        DateTimeOffset.FromUnixTimeSeconds(valor.ToUnixTimeSeconds());
}