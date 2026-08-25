using System.Text.Json.Serialization;

namespace Wallet.Domain.Enums;

public enum Categoria
{
    [JsonStringEnumMemberName("adulto")]
    Adulto,

    [JsonStringEnumMemberName("juvenil")]
    Juvenil,

    [JsonStringEnumMemberName("niño")]
    Nino
}