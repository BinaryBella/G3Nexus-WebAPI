using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using G3NexusBackend.Models;

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

        // Configure the relationship
        builder.HasOne(p => p.Project)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior if needed
    }
}