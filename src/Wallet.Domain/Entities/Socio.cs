using Wallet.Domain.Enums;

namespace Wallet.Domain.Entities;

public class Socio
{
    private Socio() { }

    public Socio(string nombre, string apellido, string dni, Categoria categoria, string fotoUrl)
    {
        Did = $"did:example:{Guid.NewGuid()}";
        Nombre = nombre;
        Apellido = apellido;
        Dni = dni;
        Categoria = categoria;
        FotoUrl = fotoUrl;
    }

    public int Id { get; private set; }
    public string Did { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string Apellido { get; private set; } = null!;
    public string Dni { get; private set; } = null!;
    public Categoria Categoria { get; private set; }
    public string FotoUrl { get; private set; } = null!;

    public string NumeroSocio => Id.ToString("D6");
}