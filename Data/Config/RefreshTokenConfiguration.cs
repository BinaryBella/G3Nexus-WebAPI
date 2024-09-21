using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using G3NexusBackend.Models;

namespace G3NexusBackend.Data.Configurations
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
                .HasMaxLength(256);

            // Configure Expires as a required field
            builder.Property(rt => rt.Expires)
                .IsRequired();

            // Configure IsRevoked with default value false
            builder.Property(rt => rt.IsRevoked)
                .HasDefaultValue(false);

            // Table name (optional, if you want to specify a custom table name)
            builder.ToTable("RefreshTokens");
        }
    }
}