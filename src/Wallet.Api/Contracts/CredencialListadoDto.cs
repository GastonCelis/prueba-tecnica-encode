using System.Text.Json;
using System.Text.Json.Nodes;
using Wallet.Domain.Entities;

namespace Wallet.Api.Contracts;

public sealed class CredencialListadoDto
{
    public required Guid Id { get; init; }
    public required string Nombre { get; init; }
    public required string Apellido { get; init; }
    public required string Dni { get; init; }
    public required string NumeroSocio { get; init; }
    public required string Categoria { get; init; }
    public required string Foto { get; init; }
    public required DateTimeOffset ValidFrom { get; init; }
    public required DateTimeOffset ValidUntil { get; init; }
    public required int CredentialStatus { get; init; }
    public required JsonNode Vc { get; init; }

    public static CredencialListadoDto Desde(Credential credential)
    {
        var vc = JsonNode.Parse(credential.VcJson)!;
        var subject = vc["credentialSubject"]!;

        return new CredencialListadoDto
        {
            Id = credential.Id,
            Nombre = subject["nombre"]!.GetValue<string>(),
            Apellido = subject["apellido"]!.GetValue<string>(),
            Dni = subject["dni"]!.GetValue<string>(),
            NumeroSocio = subject["numeroSocio"]!.GetValue<string>(),
            Categoria = subject["categoria"]!.GetValue<string>(),
            Foto = subject["foto"]!.GetValue<string>(),
            ValidFrom = credential.ValidFrom,
            ValidUntil = credential.ValidUntil,
            CredentialStatus = (int)credential.Status,
            Vc = vc
        };
    }
}