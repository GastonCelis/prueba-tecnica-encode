using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Persistence;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("credentials");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.VcJson).IsRequired();
        builder.Property(c => c.ValidFrom).IsRequired();
        builder.Property(c => c.ValidUntil).IsRequired();
        builder.Property(c => c.Status).IsRequired();

        builder.HasOne(c => c.Socio)
               .WithMany()
               .HasForeignKey(c => c.SocioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.ValidFrom);
    }
}