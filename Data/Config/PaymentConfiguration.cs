using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table name
        builder.ToTable("Payments");

        // Primary key
        builder.HasKey(p => p.PaymentId);

        // Properties
        builder.Property(p => p.PaymentAmount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.PaymentDescription)
            .HasMaxLength(255);

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.Attachment)
            .HasMaxLength(255);

        // Foreign Key for Project
        builder.HasOne(p => p.Project)
            .WithMany()
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
