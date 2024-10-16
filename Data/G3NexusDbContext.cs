﻿using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

public class G3NexusDbContext : DbContext
{
    public G3NexusDbContext(DbContextOptions<G3NexusDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<Bug> Bugs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Verification> Verifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new RequirementConfiguration());
        modelBuilder.ApplyConfiguration(new BugConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationConfiguration());
    }
}
