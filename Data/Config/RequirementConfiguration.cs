using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequirementConfiguration : IEntityTypeConfiguration<Requirement>
{
    public void Configure(EntityTypeBuilder<Requirement> builder)
    {
        // Table name
        builder.ToTable("Requirements");

        // Primary key
        builder.HasKey(r => r.RequirementId);

        // Properties
        builder.Property(r => r.RequirementTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.RequirementDescription)
            .HasMaxLength(1000);

        builder.Property(r => r.Attachment)
            .HasMaxLength(255);

        // Foreign Key for Project
        builder.HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
