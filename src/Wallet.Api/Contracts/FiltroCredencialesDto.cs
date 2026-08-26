using System.ComponentModel.DataAnnotations;

namespace Wallet.Api.Contracts;

public sealed class FiltroCredencialesDto
{
    /// <summary>Busca coincidencias parciales en nombre, apellido, DNI o número de socio.</summary>
    [StringLength(100)]
    public string? Busqueda { get; set; }

    [RegularExpression("^(adulto|juvenil|niño)$",
        ErrorMessage = "La categoría debe ser adulto, juvenil o niño.")]
    public string? Categoria { get; set; }

    [Range(0, 2, ErrorMessage = "El estado debe ser 0 (activa), 1 (revocada) o 2 (suspendida).")]
    public int? Estado { get; set; }
}