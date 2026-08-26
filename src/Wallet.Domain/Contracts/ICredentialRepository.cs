using Wallet.Domain.Entities;

namespace Wallet.Domain.Contracts;

public interface ICredentialRepository
{
    Task<Socio> AgregarSocioAsync(Socio socio, CancellationToken ct);
    Task AgregarCredencialAsync(Credential credential, CancellationToken ct);
    Task<IReadOnlyList<Credential>> ListarAsync(string? busqueda, int? estado, CancellationToken ct);
}