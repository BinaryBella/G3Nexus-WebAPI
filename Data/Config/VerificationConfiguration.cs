using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
{
    public void Configure(EntityTypeBuilder<Verification> builder)
    {
        // Table name
        builder.ToTable("Verifications");

        // Primary key
        builder.HasKey(v => v.VId);

        builder.Property(v => v.VerificationCode)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(v => v.ExpiryDate)
            .IsRequired();
    }
}
