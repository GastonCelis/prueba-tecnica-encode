using Wallet.Domain.Enums;

namespace Wallet.Domain.Contracts;

public sealed record AltaCredencialRequest(
    string Nombre,
    string Apellido,
    string Dni,
    Categoria Categoria,
    string Foto);