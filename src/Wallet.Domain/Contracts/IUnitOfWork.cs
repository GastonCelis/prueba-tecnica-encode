namespace Wallet.Domain.Contracts;

public interface IUnitOfWork
{
    Task<ITransaccion> IniciarTransaccionAsync(CancellationToken ct);
}

public interface ITransaccion : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}