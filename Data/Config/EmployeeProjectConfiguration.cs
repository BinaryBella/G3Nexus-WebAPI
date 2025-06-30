using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        // Composite primary key
        builder.HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

        // Relationships
        builder.HasOne(ep => ep.Employee)
            .WithMany(e => e.EmployeeProjects)
            .HasForeignKey(ep => ep.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ep => ep.Project)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(ep => ep.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: Configure table name and schema
        builder.ToTable("EmployeeProjects"); // Optional: rename table
    }
}