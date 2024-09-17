using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class BugConfiguration : IEntityTypeConfiguration<Bug>
{
    public void Configure(EntityTypeBuilder<Bug> builder)
    {
        // Table name
        builder.ToTable("Bugs");

        // Primary key
        builder.HasKey(b => b.BugId);

        // Properties
        builder.Property(b => b.BugTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Severity)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.BugDescription)
            .HasMaxLength(1000);

        builder.Property(b => b.Attachment)
            .HasMaxLength(255);

        // Foreign Key for Project
        builder.HasOne(b => b.Project)
            .WithMany()
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
