using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        // Table name
        builder.ToTable("Quotations");

        // Primary key
        builder.HasKey(q => q.QuotationId);

        // Properties
        builder.Property(q => q.CreatedDate)
            .IsRequired();

        builder.Property(q => q.Type)
            .HasMaxLength(50);

        builder.Property(q => q.TotalCost)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

    }
}
