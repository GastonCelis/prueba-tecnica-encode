using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wallet.Infrastructure.Persistence;

#nullable disable

namespace Wallet.Infrastructure.Migrations
{
    [DbContext(typeof(WalletDbContext))]
    [Migration("20260825194749_Inicial")]
    partial class Inicial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.11");

            modelBuilder.Entity("Wallet.Domain.Entities.Credential", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("SocioId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("ValidFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.Property<string>("VcJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SocioId");

                    b.HasIndex("ValidFrom");

                    b.ToTable("credentials", (string)null);
                });

            modelBuilder.Entity("Wallet.Domain.Entities.Socio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Did")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Dni")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("FotoUrl")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("TEXT");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Did")
                        .IsUnique();

                    b.HasIndex("Dni")
                        .IsUnique();

                    b.ToTable("socios", (string)null);
                });

            modelBuilder.Entity("Wallet.Domain.Entities.Credential", b =>
                {
                    b.HasOne("Wallet.Domain.Entities.Socio", "Socio")
                        .WithMany()
                        .HasForeignKey("SocioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Socio");
                });
#pragma warning restore 612, 618
        }
    }
}
