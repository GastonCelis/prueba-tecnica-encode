using Wallet.Domain.Contracts;
using Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Infrastructure.Persistence;

public sealed class CredentialRepository : ICredentialRepository
{
    private readonly WalletDbContext _db;

    public CredentialRepository(WalletDbContext db) => _db = db;

    public async Task<Socio> AgregarSocioAsync(Socio socio, CancellationToken ct)
    {
        _db.Socios.Add(socio);
        await _db.SaveChangesAsync(ct);
        return socio;
    }

    public async Task AgregarCredencialAsync(Credential credential, CancellationToken ct)
    {
        _db.Credentials.Add(credential);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Credential>> ListarAsync(
        string? busqueda, int? estado, CancellationToken ct)
    {
        var query = _db.Credentials
            .AsNoTracking()
            .Include(c => c.Socio)
            .AsQueryable();

        if (estado is not null)
            query = query.Where(c => (int)c.Status == estado);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.Socio.Nombre, $"%{termino}%") ||
                EF.Functions.Like(c.Socio.Apellido, $"%{termino}%") ||
                EF.Functions.Like(c.Socio.Dni, $"%{termino}%"));
        }

        var credenciales = await query.ToListAsync(ct);

        return credenciales
            .OrderByDescending(c => c.ValidFrom)
            .ToList();
    }
}