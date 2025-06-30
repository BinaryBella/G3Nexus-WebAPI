using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class ClientProjectConfiguration : IEntityTypeConfiguration<ClientProject>
{
    public void Configure(EntityTypeBuilder<ClientProject> builder)
    {
        builder.HasKey(cp => new { cp.ClientId, cp.ProjectId });

        builder.HasOne(cp => cp.Client)
            .WithMany(c => c.ClientProjects)
            .HasForeignKey(cp => cp.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.Project)
            .WithMany(p => p.ClientProjects)
            .HasForeignKey(cp => cp.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("ClientProjects");
    }
}