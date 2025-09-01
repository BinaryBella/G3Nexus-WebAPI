using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(G3NexusDbContext context)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();

                // Check if a company admin already exists
                bool hasCompanyAdmin = await context.Employees!
                    .AnyAsync(e => e.Role == "COMPANY_ADMIN");

                if (hasCompanyAdmin)
                {
                    Console.WriteLine("Company admin already exists. Skipping seeding.");
                    return;
                }

                var admin = new Employee
                {
                    Name = "A. Chathushi Kavindya",
                    ContactNo = "+1-555-1001",
                    Email = "chathushikavindya09@gmail.com",
                    Address = "100 Developer Lane",
                    Password = BCrypt.Net.BCrypt.HashPassword("12345678"),
                    Role = "COMPANY_ADMIN",
                    IsActive = true
                };

                await context.Employees!.AddAsync(admin);
                await context.SaveChangesAsync();

                Console.WriteLine("Default Company Admin seeded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding error: {ex.Message}");
            }
        }
    }
}
