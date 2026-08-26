using Wallet.Domain.Contracts;
using Wallet.Domain.Entities;

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
}