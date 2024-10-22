using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RequirementConfiguration : IEntityTypeConfiguration<Requirement>
{
    public void Configure(EntityTypeBuilder<Requirement> builder)
    {
        // Table name
        builder.ToTable("Requirements");

        // Primary Key
        builder.HasKey(r => r.RequirementId);

        // Properties
        builder.Property(r => r.RequirementTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Priority)
            .IsRequired()
            .HasMaxLength(50); // Define based on priority categories like "High", "Medium", "Low", etc.

        builder.Property(r => r.RequirementDescription)
            .IsRequired()
            .HasMaxLength(500); // Adjust length according to your requirements

        builder.Property(r => r.Attachment)
            .HasMaxLength(200); // Assuming this is a URL or file path

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasMaxLength(50); // Status such as "Active", "Inactive", etc.

        // Foreign Keys and Relationships
        builder.HasOne(r => r.Client)
            .WithMany(c => c.Requirements)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as per your needs

        builder.HasOne(r => r.Project)
            .WithMany(p => p.Requirements)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as per your needs
    }
}