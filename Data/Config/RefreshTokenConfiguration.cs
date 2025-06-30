using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Configure primary key
            builder.HasKey(rt => rt.Id);

            // Configure Token as required and with max length
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)");

            // Configure Expires as a required field
            builder.Property(rt => rt.ExpiryDate)
                .IsRequired();

            // Configure IsRevoked with default value false
            builder.Property(rt => rt.IsRevoked)
                .HasDefaultValue(false);

            // Table name (optional, if you want to specify a custom table name)
            builder.ToTable("RefreshTokens");
        }
    }
}