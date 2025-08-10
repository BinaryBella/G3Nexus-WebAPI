using G3NexusBackend.Data.Config;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

public class G3NexusDbContext : DbContext
{
    public G3NexusDbContext(DbContextOptions<G3NexusDbContext> options) : base(options) { }
    public DbSet<Client>? Clients { get; set; }
    public DbSet<Employee>? Employees { get; set; }
    public DbSet<Project>? Projects { get; set; }
    public DbSet<Requirement>? Requirements { get; set; }
    public DbSet<Bug>? Bugs { get; set; }
    public DbSet<TermsConditions>? TermsConditions { get; set; }
    public DbSet<Payment>? Payments { get; set; }
    public DbSet<RefreshToken>? RefreshTokens { get; set; }
    public DbSet<Verification>? Verifications { get; set; }
    public DbSet<Company>? Companies { get; set; }
    public DbSet<EmployeeProject>? EmployeeProjects { get; set; }
    public DbSet<QuotationCost>? QuotationCosts { get; set; }
    public DbSet<ProjectTermsConditions>? ProjectTermsConditions { get; set; }

    public DbSet<Quotation>? Quotations { get; set; }
    public DbSet<QuotationRequirement>? QuotationRequirements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-one relationship between Project and QuotationCost
        modelBuilder.Entity<Project>()
            .HasOne(p => p.QuotationCost)
            .WithOne(q => q.Project)
            .HasForeignKey<QuotationCost>(q => q.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Apply configurations
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new RequirementConfiguration());
        modelBuilder.ApplyConfiguration(new BugConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TermsConditionsConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeProjectConfiguration());
        modelBuilder.ApplyConfiguration(new QuotationCostsConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectTermsConditionsConfiguration());
    }
}
