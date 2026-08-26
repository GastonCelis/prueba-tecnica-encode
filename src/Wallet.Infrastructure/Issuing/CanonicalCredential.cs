using System.Text.Json.Serialization;
using Wallet.Domain.Enums;

namespace Wallet.Infrastructure.Issuing;

internal sealed record CanonicalCredential
{
    [JsonPropertyName("credentialStatus"), JsonPropertyOrder(1)]
    public required int CredentialStatus { get; init; }

    [JsonPropertyName("credentialSubject"), JsonPropertyOrder(2)]
    public required CanonicalSubject CredentialSubject { get; init; }

    [JsonPropertyName("id"), JsonPropertyOrder(3)]
    public required string Id { get; init; }

    [JsonPropertyName("issuer"), JsonPropertyOrder(4)]
    public required string Issuer { get; init; }

    [JsonPropertyName("type"), JsonPropertyOrder(5)]
    public required IReadOnlyList<string> Type { get; init; }

    [JsonPropertyName("validFrom"), JsonPropertyOrder(6)]
    public required string ValidFrom { get; init; }

    [JsonPropertyName("validUntil"), JsonPropertyOrder(7)]
    public required string ValidUntil { get; init; }
}

internal sealed record CanonicalSubject
{
    [JsonPropertyName("apellido"), JsonPropertyOrder(1)]
    public required string Apellido { get; init; }

    [JsonPropertyName("categoria"), JsonPropertyOrder(2)]
    public required Categoria Categoria { get; init; }

    [JsonPropertyName("dni"), JsonPropertyOrder(3)]
    public required string Dni { get; init; }

    [JsonPropertyName("foto"), JsonPropertyOrder(4)]
    public required string Foto { get; init; }

    [JsonPropertyName("id"), JsonPropertyOrder(5)]
    public required string Id { get; init; }

    [JsonPropertyName("nombre"), JsonPropertyOrder(6)]
    public required string Nombre { get; init; }

    [JsonPropertyName("numeroSocio"), JsonPropertyOrder(7)]
    public required string NumeroSocio { get; init; }
}