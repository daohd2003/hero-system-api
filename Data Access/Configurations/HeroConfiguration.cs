using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class HeroConfiguration : IEntityTypeConfiguration<Hero>
    {
        public void Configure(EntityTypeBuilder<Hero> builder)
        {
            builder.ToTable(nameof(Hero));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(true);

            builder.Property(x => x.SuperPower)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Level)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.RefreshToken)
                .HasMaxLength(256);

            builder.Property(x => x.RefreshTokenExpiryTime);
        }
    }
}
