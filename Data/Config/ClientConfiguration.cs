using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace G3NexusBackend.Data.Config;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Table name
        builder.ToTable("Clients");

        // Primary Key
        builder.HasKey(c => c.ClientId);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust based on name length requirements

        builder.Property(c => c.ContactNo)
            .IsRequired()
            .HasMaxLength(20); // Assuming phone number length is at most 20 characters

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100); // Email addresses typically max out at 254 characters but adjust to 100 as reasonable limit

        builder.Property(c => c.Address)
            .HasMaxLength(200); // Adjust the length for the address field

        builder.Property(c => c.Password)
            .IsRequired()
            .HasMaxLength(100); // Ensure sufficient length for storing hashed passwords

        builder.Property(c => c.Role)
            .IsRequired()
            .HasMaxLength(50); // Role such as "Admin", "User", "Client"

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasMaxLength(50); // For status like "Active", "Inactive", etc.

        // Relationships

        // One-to-many relationship with Requirement
        builder.HasMany(c => c.Requirements)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete

        // One-to-many relationship with Bug
        builder.HasMany(c => c.Bugs)
            .WithOne(b => b.Client)
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete


        builder.HasMany(q => q.Quotations)
            .WithOne(c => c.Client)
            .HasForeignKey(q => q.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // // Foreign Keys and Relationships
        // builder.HasOne(r => r.Company)
        //     .WithMany(c => c.Clients)
        //     .HasForeignKey(r => r.CompanyId)
        //     .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as per your needs
    }
}