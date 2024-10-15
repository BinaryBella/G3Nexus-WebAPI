using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectEmployeeClientConfiguration : IEntityTypeConfiguration<ProjectEmployeeClient>
{
    public void Configure(EntityTypeBuilder<ProjectEmployeeClient> builder)
    {
        // Table name
        builder.ToTable("ProjectEmployeeClients");

        // Composite Primary Key (ProjectId, EmployeeId, ClientId)
        builder.HasKey(pec => new { pec.ProjectId, pec.EmployeeId, pec.ClientId });

        // Foreign Key: ProjectId
        builder.HasOne(pec => pec.Project)
            .WithMany(p => p.ProjectEmployeeClients)
            .HasForeignKey(pec => pec.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete when the related project is deleted

        // Foreign Key: EmployeeId
        builder.HasOne(pec => pec.Employee)
            .WithMany(e => e.ProjectEmployeeClients)
            .HasForeignKey(pec => pec.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete when the related employee is deleted

        // Foreign Key: ClientId
        builder.HasOne(pec => pec.Client)
            .WithMany(c => c.ProjectEmployeeClients)
            .HasForeignKey(pec => pec.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete when the related client is deleted
    }
}