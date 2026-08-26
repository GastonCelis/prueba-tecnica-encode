using Wallet.Domain.Enums;

namespace Wallet.Domain.Contracts;

public sealed record CredentialSubject(
    string Id,
    string Nombre,
    string Apellido,
    string Dni,
    string NumeroSocio,
    Categoria Categoria,
    string Foto);