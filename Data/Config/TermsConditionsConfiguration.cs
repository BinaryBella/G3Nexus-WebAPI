using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class TermsConditionsConfiguration : IEntityTypeConfiguration<TermsConditions>
{
    public void Configure(EntityTypeBuilder<TermsConditions> builder)
    {
        builder.ToTable("TermsConditions");

        builder.HasKey(tc => tc.TCId);

        builder.Property(tc => tc.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(tc => tc.UpdatedDate)
            .IsRequired();

        builder.Property(tc => tc.IsActive)
            .IsRequired();
    }
}