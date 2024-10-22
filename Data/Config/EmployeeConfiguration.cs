using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table name
        builder.ToTable("Employees");

        // Primary Key
        builder.HasKey(e => e.EmployeeId);

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust length based on name requirements

        builder.Property(e => e.ContactNo)
            .IsRequired()
            .HasMaxLength(20); // Phone number with max length of 20 characters

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100); // Email with max length of 100 characters

        builder.Property(e => e.Address)
            .HasMaxLength(200); // Address with max length of 200 characters

        builder.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(100); // Storing hashed password

        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(50); // 
        
        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasMaxLength(50); // For status like "Active", "Inactive", etc.

        // Relationships

        // One-to-many relationship with ProjectEmployeeClient
        builder.HasMany(e => e.ProjectEmployeeClients)
            .WithOne(pec => pec.Employee)
            .HasForeignKey(pec => pec.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if employee is deleted
    }
}