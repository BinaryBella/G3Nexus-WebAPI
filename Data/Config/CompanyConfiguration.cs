using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
            builder.ToTable("Company");

            builder.HasKey(c => c.CompanyId);

            builder.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .IsRequired(); 

            // One-to-many relationship with Clients
            builder.HasMany(c => c.Clients)
                .WithOne(r => r.Company)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}