using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using G3NexusBackend.Models;

public class QuotationCostsConfiguration : IEntityTypeConfiguration<QuotationCost>
{
    public void Configure(EntityTypeBuilder<QuotationCost> builder)
    {
        builder.ToTable("QuotationCosts");

        builder.HasKey(q => q.Id);

        builder.HasOne(q => q.Project)
            .WithOne(p => p.QuotationCost)
            .HasForeignKey<QuotationCost>(q => q.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(q => q.AdvancePayment)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(q => q.DevelopmentCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(q => q.HostingAndDomain)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(q => q.SSLCertificate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(q => q.DeploymentCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
