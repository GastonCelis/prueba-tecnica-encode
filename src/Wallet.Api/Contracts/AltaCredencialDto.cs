using System.ComponentModel.DataAnnotations;

namespace Wallet.Api.Contracts;

public sealed class AltaCredencialDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 1)]
    public string Apellido { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\d{7,9}$",
        ErrorMessage = "El DNI debe contener entre 7 y 9 dígitos.")]
    public string Dni { get; set; } = string.Empty;

    [Required, RegularExpression("^(adulto|juvenil|niño)$",
        ErrorMessage = "La categoría debe ser adulto, juvenil o niño.")]
    public string Categoria { get; set; } = string.Empty;

    [Required, Url]
    public string Foto { get; set; } = string.Empty;
}