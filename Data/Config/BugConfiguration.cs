using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BugConfiguration : IEntityTypeConfiguration<Bug>
{
    public void Configure(EntityTypeBuilder<Bug> builder)
    {
        // Table name
        builder.ToTable("Bugs");

        // Primary Key
        builder.HasKey(b => b.BugId);

        // Properties
        builder.Property(b => b.BugTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Severity)
            .IsRequired()
            .HasMaxLength(50); // Adjust length based on your requirements (e.g., "Critical", "High", etc.)

        builder.Property(b => b.BugDescription)
            .IsRequired()
            .HasMaxLength(500); // Adjust description length as per your use case

        builder.Property(b => b.Attachment)
            .HasMaxLength(200); // Assuming this is a URL or file path

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasMaxLength(50); // For status like "Active", "Inactive", etc.

        // Foreign Keys and Relationships
        builder.HasOne(b => b.Client)
            .WithMany(c => c.Bugs)
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed

        builder.HasOne(b => b.Project)
            .WithMany(p => p.Bugs)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed
    }
}