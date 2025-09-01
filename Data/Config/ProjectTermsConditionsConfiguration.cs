using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class ProjectTermsConditionsConfiguration : IEntityTypeConfiguration<ProjectTermsConditions>
{
    public void Configure(EntityTypeBuilder<ProjectTermsConditions> builder)
    {
        builder.HasKey(pt => new { pt.ProjectId, pt.TCId }); // Composite Primary Key

        builder
            .HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTermsConditions)
            .HasForeignKey(pt => pt.ProjectId);

        builder
            .HasOne(pt => pt.TermsConditions)
            .WithMany()
            .HasForeignKey(pt => pt.TCId);

        builder.Property(pt => pt.IsChecked)
            .IsRequired();
    }
}