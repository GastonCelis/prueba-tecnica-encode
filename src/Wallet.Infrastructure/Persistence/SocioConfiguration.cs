using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Persistence;

public class SocioConfiguration : IEntityTypeConfiguration<Socio>
{
    public void Configure(EntityTypeBuilder<Socio> builder)
    {
        builder.ToTable("socios");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Did).IsRequired().HasMaxLength(64);
        builder.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Apellido).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Dni).IsRequired().HasMaxLength(20);
        builder.Property(s => s.FotoUrl).IsRequired().HasMaxLength(2048);
        builder.Property(s => s.Categoria).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(s => s.Dni).IsUnique();
        builder.HasIndex(s => s.Did).IsUnique();

        builder.Ignore(s => s.NumeroSocio);
    }
}