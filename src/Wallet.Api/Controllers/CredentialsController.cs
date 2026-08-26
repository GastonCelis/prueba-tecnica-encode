using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.Contracts;
using Wallet.Domain.Contracts;
using Wallet.Domain.Enums;
using Wallet.Domain.Services;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CredentialsController : ControllerBase
{
    private readonly EmitirCredencialService _emitir;

    public CredentialsController(EmitirCredencialService emitir) => _emitir = emitir;

    [HttpPost]
    public async Task<IActionResult> Alta(
        [FromBody] AltaCredencialDto dto, CancellationToken ct)
    {
        var categoria = dto.Categoria switch
        {
            "adulto" => Categoria.Adulto,
            "juvenil" => Categoria.Juvenil,
            "niño" => Categoria.Nino,
            _ => throw new InvalidOperationException("Categoría no válida.")
        };

        var request = new AltaCredencialRequest(
            dto.Nombre.Trim(), dto.Apellido.Trim(), dto.Dni.Trim(),
            categoria, dto.Foto.Trim());

        try
        {
            var vc = await _emitir.EjecutarAsync(request, ct);

            return CreatedAtAction(nameof(Alta), new { id = vc.RawId }, new
            {
                id = vc.Id,
                numeroSocio = vc.CredentialSubject.NumeroSocio,
                validFrom = vc.ValidFrom,
                validUntil = vc.ValidUntil
            });
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                title: "No se pudo emitir la credencial",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
        catch (DbUpdateException)
        {
            return Problem(
                title: "No se pudo emitir la credencial",
                detail: "Ya existe un socio registrado con ese DNI.",
                statusCode: StatusCodes.Status409Conflict);
        }
    }
}