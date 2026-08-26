using Wallet.Domain.Contracts;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;

namespace Wallet.Domain.Services;

/// <summary>
/// Rol Tenant: recupera las credenciales emitidas para su presentación
/// en el listado (UC02), con filtros opcionales.
/// </summary>
public sealed class ListarCredencialesService
{
    private readonly ICredentialRepository _repo;

    public ListarCredencialesService(ICredentialRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Credential>> EjecutarAsync(
        string? busqueda, Categoria? categoria, int? estado, CancellationToken ct)
    {
        var credenciales = await _repo.ListarAsync(busqueda, estado, ct);

        if (categoria is not null)
            credenciales = credenciales
                .Where(c => c.Socio.Categoria == categoria)
                .ToList();

        return credenciales;
    }
}