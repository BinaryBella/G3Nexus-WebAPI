using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table name
        builder.ToTable("Projects");

        // Primary key
        builder.HasKey(p => p.ProjectId);

        // Properties
        builder.Property(p => p.ProjectName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.ProjectType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ProjectSize)
            .HasMaxLength(50);

        builder.Property(p => p.CreationDate)
            .IsRequired();

        builder.Property(p => p.ProjectDescription)
            .HasMaxLength(1000);

        builder.Property(p => p.EstimatedBudget)
            .HasColumnType("decimal(18, 2)");

        builder.Property(p => p.TotalBudget)
            .HasColumnType("decimal(18, 2)");
        
        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasMaxLength(50); 
    }
}