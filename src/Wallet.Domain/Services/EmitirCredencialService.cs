using Wallet.Domain.Contracts;
using Wallet.Domain.Entities;

namespace Wallet.Domain.Services;

/// <summary>
/// Rol Tenant: arma el credentialSubject del socio, solicita la emisión
/// al Issuer y persiste la credencial firmada. Toda la operación ocurre
/// dentro de una transacción: si la firma falla, no se persiste nada.
/// </summary>
public sealed class EmitirCredencialService
{
    private readonly ICredentialRepository _repo;
    private readonly ICredentialIssuer _issuer;
    private readonly IUnitOfWork _uow;

    public EmitirCredencialService(
        ICredentialRepository repo, ICredentialIssuer issuer, IUnitOfWork uow)
    {
        _repo = repo;
        _issuer = issuer;
        _uow = uow;
    }

    public async Task<VerifiableCredential> EjecutarAsync(
        AltaCredencialRequest request, CancellationToken ct)
    {
        await using var transaccion = await _uow.IniciarTransaccionAsync(ct);

        var socio = new Socio(
            request.Nombre, request.Apellido, request.Dni,
            request.Categoria, request.Foto);

        await _repo.AgregarSocioAsync(socio, ct);

        var subject = new CredentialSubject(
            Id: socio.Did,
            Nombre: socio.Nombre,
            Apellido: socio.Apellido,
            Dni: socio.Dni,
            NumeroSocio: socio.NumeroSocio,
            Categoria: socio.Categoria,
            Foto: socio.FotoUrl);

        var vc = _issuer.Emitir(subject);

        var credencial = new Credential(
            vc.RawId, socio.Id, _issuer.Serializar(vc), vc.ValidFrom, vc.ValidUntil);

        await _repo.AgregarCredencialAsync(credencial, ct);

        await transaccion.CommitAsync(ct);

        return vc;
    }
}