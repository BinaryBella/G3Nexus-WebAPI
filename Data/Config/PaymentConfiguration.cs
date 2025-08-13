using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Define the primary key
        builder.HasKey(p => p.PaymentId);

        // Configure properties
        builder.Property(p => p.PaymentAmount)
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .IsRequired()
            .HasMaxLength(50); // Set appropriate max length

        builder.Property(p => p.PaymentDescription)
            .HasMaxLength(200); // Set appropriate max length

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.Attachment)
            .HasMaxLength(255); // Set appropriate max length

        // Configure the relationships
        builder.HasOne(p => p.Project)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior if needed

        builder.HasOne(p => p.Client)
            .WithMany() // Assuming Client doesn't have a navigation property back to Payments
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete for client payments
    }
}