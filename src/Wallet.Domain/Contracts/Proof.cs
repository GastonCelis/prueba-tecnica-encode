namespace Wallet.Domain.Contracts;

public sealed record Proof(
    string Type,
    DateTimeOffset Created,
    string VerificationMethod,
    string ProofValue);