using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Wallet.Domain.Contracts;

namespace Wallet.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WalletDbContext _db;

    public UnitOfWork(WalletDbContext db) => _db = db;

    public async Task<ITransaccion> IniciarTransaccionAsync(CancellationToken ct) =>
        new TransaccionEf(await _db.Database.BeginTransactionAsync(ct));

    private sealed class TransaccionEf : ITransaccion
    {
        private readonly IDbContextTransaction _inner;

        public TransaccionEf(IDbContextTransaction inner) => _inner = inner;

        public Task CommitAsync(CancellationToken ct) => _inner.CommitAsync(ct);

        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }
}