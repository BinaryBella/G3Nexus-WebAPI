using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using G3NexusBackend.Models;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.UserId);

        // Properties
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ContactNo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.EmailAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Password)
            .IsRequired();

        // Role property
        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);
    }
}
