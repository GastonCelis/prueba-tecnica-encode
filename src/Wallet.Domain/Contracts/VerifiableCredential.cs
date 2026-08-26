using Wallet.Domain.Enums;

namespace Wallet.Domain.Contracts;

public sealed record VerifiableCredential(
    Guid RawId,
    string Id,
    IReadOnlyList<string> Type,
    string Issuer,
    CredentialSubject CredentialSubject,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    CredentialStatus CredentialStatus,
    Proof Proof);