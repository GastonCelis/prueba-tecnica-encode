namespace Wallet.Domain.Contracts;

public interface ICredentialIssuer
{
    VerifiableCredential Emitir(CredentialSubject subject);
    string Serializar(VerifiableCredential vc);
}