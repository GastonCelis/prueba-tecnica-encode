using Wallet.Domain.Enums;

namespace Wallet.Domain.Entities;

public class Credential
{
    private Credential() { }

    public Credential(Guid id, int socioId, string vcJson,
                      DateTimeOffset validFrom, DateTimeOffset validUntil)
    {
        Id = id;
        SocioId = socioId;
        VcJson = vcJson;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        Status = CredentialStatus.Active;
    }

    public Guid Id { get; private set; }
    public int SocioId { get; private set; }
    public Socio Socio { get; private set; } = null!;
    public string VcJson { get; private set; } = null!;
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset ValidUntil { get; private set; }
    public CredentialStatus Status { get; private set; }
}