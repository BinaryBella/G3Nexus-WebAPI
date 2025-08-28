using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(G3NexusDbContext context, bool forceReseed = false)
        {
            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Check if data already exists to avoid duplicates (unless force reseed is true)
                if (!forceReseed && await context.Companies!.AnyAsync())
                {
                    Console.WriteLine("Database already contains data. Skipping seeding.");
                    return; // Database has been seeded
                }

                // If force reseed, clear existing data first
                if (forceReseed)
                {
                    await ClearExistingData(context);
                }

                if (!SeedConfiguration.SeedTestData)
                {
                    Console.WriteLine("Test data seeding is disabled in configuration.");
                    return;
                }

                // Seed Companies
                var companies = new List<Company>
                {
                    new Company
                    {
                        CompanyName = "TechCorp Solutions",
                        Address = "123 Tech Street, Silicon Valley, CA",
                        IsActive = true
                    },
                    new Company
                    {
                        CompanyName = "Digital Innovations Ltd",
                        Address = "456 Innovation Ave, New York, NY",
                        IsActive = true
                    },
                    new Company
                    {
                        CompanyName = "Global Software Systems",
                        Address = "789 Global Blvd, Austin, TX",
                        IsActive = true
                    }
                };

                await context.Companies!.AddRangeAsync(companies);
                await context.SaveChangesAsync();

                // Seed Clients
                var clients = new List<Client>
                {
                    new Client
                    {
                        Name = "John Smith",
                        ContactNo = "+1-555-0101",
                        Email = "teamup776@gmail.com",
                        Address = "101 Business Park, CA",
                        Password = BCrypt.Net.BCrypt.HashPassword("test"),
                        Role = "CLIENT_ADMIN",
                        IsActive = true,
                        CompanyId = companies[0].CompanyId
                    },
                    new Client
                    {
                        Name = "Sarah Johnson",
                        ContactNo = "+1-555-0102",
                        Email = "sarah.johnson@digital.com",
                        Address = "202 Corporate Center, NY",
                        Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        Role = "CLIENT_USER",
                        IsActive = true,
                        CompanyId = companies[1].CompanyId
                    },
                    new Client
                    {
                        Name = "Michael Brown",
                        ContactNo = "+1-555-0103",
                        Email = "michael.brown@global.com",
                        Address = "303 Enterprise Plaza, TX",
                        Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        Role = "CLIENT_ADMIN",
                        IsActive = true,
                        CompanyId = companies[2].CompanyId
                    }
                };

                await context.Clients!.AddRangeAsync(clients);
                await context.SaveChangesAsync();

                // Seed Employees
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        Name = "Alice Developer",
                        ContactNo = "+1-555-1001",
                        Email = "chathushikavindya09@gmail.com",
                        Address = "100 Developer Lane",
                        Password = BCrypt.Net.BCrypt.HashPassword("123"),
                        Role = "COMPANY_ADMIN",
                        IsActive = true
                    },
                    new Employee
                    {
                        Name = "Bob Architect",
                        ContactNo = "+1-555-1002",
                        Email = "bob.arch@g3nexus.com",
                        Address = "200 Architect Avenue",
                        Password = BCrypt.Net.BCrypt.HashPassword("ArchPassword123!"),
                        Role = "COMPANY_ADMIN",
                        IsActive = true
                    },
                    new Employee
                    {
                        Name = "Carol Tester",
                        ContactNo = "+1-555-1003",
                        Email = "carol.test@g3nexus.com",
                        Address = "300 QA Street",
                        Password = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
                        Role = "COMPANY_DEVELOPER",
                        IsActive = true
                    },
                    new Employee
                    {
                        Name = "David Manager",
                        ContactNo = "+1-555-1004",
                        Email = "david.mgr@g3nexus.com",
                        Address = "400 Management Road",
                        Password = BCrypt.Net.BCrypt.HashPassword("MgrPassword123!"),
                        Role = "COMPANY_DEVELOPER",
                        IsActive = true
                    }
                };

                await context.Employees!.AddRangeAsync(employees);
                await context.SaveChangesAsync();

                // Seed Terms & Conditions
                var termsConditions = new List<TermsConditions>
                {
                    new TermsConditions
                    {
                        Content = "Standard software development terms and conditions. All work will be completed according to agreed specifications and timelines.",
                        UpdatedDate = DateTime.Now.AddDays(-30),
                        IsActive = true
                    },
                    new TermsConditions
                    {
                        Content = "Premium service terms with extended support and maintenance included for 12 months post-delivery.",
                        UpdatedDate = DateTime.Now.AddDays(-15),
                        IsActive = true
                    }
                };

                await context.TermsConditions!.AddRangeAsync(termsConditions);
                await context.SaveChangesAsync();

                // Seed Projects
                var projects = new List<Project>
                {
                    new Project
                    {
                        ProjectName = "E-Commerce Platform",
                        ProjectType = "Web Application",
                        ProjectSize = "Large",
                        CreationDate = DateTime.Now.AddDays(-60),
                        ProjectDescription = "A comprehensive e-commerce platform with payment integration, inventory management, and customer portal.",
                        EstimatedBudget = 150000.00m,
                        TotalBudget = 160000.00m,
                        ActualStartDate = DateTime.Now.AddDays(-50),
                        PaymentType = "Milestone",
                        PaymentStatus = "Partial",
                        Status = "In Progress",
                        IsActive = true,
                        CompanyId = companies[0].CompanyId
                    },
                    new Project
                    {
                        ProjectName = "Mobile Banking App",
                        ProjectType = "Mobile Application",
                        ProjectSize = "Medium",
                        CreationDate = DateTime.Now.AddDays(-45),
                        ProjectDescription = "Secure mobile banking application with biometric authentication and real-time transaction processing.",
                        EstimatedBudget = 80000.00m,
                        TotalBudget = 85000.00m,
                        ActualStartDate = DateTime.Now.AddDays(-40),
                        PaymentType = "Milestone",
                        PaymentStatus = "Partial",
                        Status = "In Progress",
                        IsActive = true,
                        CompanyId = companies[1].CompanyId
                    },
                    new Project
                    {
                        ProjectName = "CRM System",
                        ProjectType = "Desktop Application",
                        ProjectSize = "Small",
                        CreationDate = DateTime.Now.AddDays(-30),
                        ProjectDescription = "Customer relationship management system with lead tracking and sales analytics.",
                        EstimatedBudget = 45000.00m,
                        TotalBudget = 50000.00m,
                        PaymentType = "Fixed",
                        PaymentStatus = "Pending",
                        Status = "Planning",
                        IsActive = true,
                        CompanyId = companies[2].CompanyId
                    }
                };

                await context.Projects!.AddRangeAsync(projects);
                await context.SaveChangesAsync();

                // Seed Employee-Project assignments
                var employeeProjects = new List<EmployeeProject>
                {
                    new EmployeeProject { EmployeeId = employees[0].EmployeeId, ProjectId = projects[0].ProjectId },
                    new EmployeeProject { EmployeeId = employees[1].EmployeeId, ProjectId = projects[0].ProjectId },
                    new EmployeeProject { EmployeeId = employees[2].EmployeeId, ProjectId = projects[0].ProjectId },
                    new EmployeeProject { EmployeeId = employees[3].EmployeeId, ProjectId = projects[0].ProjectId },
                    new EmployeeProject { EmployeeId = employees[0].EmployeeId, ProjectId = projects[1].ProjectId },
                    new EmployeeProject { EmployeeId = employees[1].EmployeeId, ProjectId = projects[1].ProjectId },
                    new EmployeeProject { EmployeeId = employees[3].EmployeeId, ProjectId = projects[2].ProjectId }
                };

                await context.EmployeeProjects!.AddRangeAsync(employeeProjects);
                await context.SaveChangesAsync();

                // Seed Project Terms & Conditions
                var projectTermsConditions = new List<ProjectTermsConditions>
                {
                    new ProjectTermsConditions { ProjectId = projects[0].ProjectId, TCId = termsConditions[0].TCId, IsChecked = true },
                    new ProjectTermsConditions { ProjectId = projects[1].ProjectId, TCId = termsConditions[1].TCId, IsChecked = true },
                    new ProjectTermsConditions { ProjectId = projects[2].ProjectId, TCId = termsConditions[0].TCId, IsChecked = false }
                };

                await context.ProjectTermsConditions!.AddRangeAsync(projectTermsConditions);
                await context.SaveChangesAsync();

                // Seed Quotation Costs
                var quotationCosts = new List<QuotationCost>
                {
                    new QuotationCost
                    {
                        ProjectId = projects[0].ProjectId,
                        AdvancePayment = 48000.00m,
                        DevelopmentCost = 120000.00m,
                        HostingAndDomain = 1200.00m,
                        SSLCertificate = 500.00m,
                        ServerCost = 2400.00m,
                        DeploymentCost = 8000.00m
                    },
                    new QuotationCost
                    {
                        ProjectId = projects[1].ProjectId,
                        AdvancePayment = 25500.00m,
                        DevelopmentCost = 65000.00m,
                        HostingAndDomain = 1000.00m,
                        SSLCertificate = 400.00m,
                        ServerCost = 1800.00m,
                        DeploymentCost = 6000.00m
                    },
                    new QuotationCost
                    {
                        ProjectId = projects[2].ProjectId,
                        AdvancePayment = 15000.00m,
                        DevelopmentCost = 38000.00m,
                        HostingAndDomain = 800.00m,
                        SSLCertificate = 300.00m,
                        ServerCost = 1200.00m,
                        DeploymentCost = 4000.00m
                    }
                };

                await context.QuotationCosts!.AddRangeAsync(quotationCosts);
                await context.SaveChangesAsync();

                // Seed Requirements
                var requirements = new List<Requirement>
                {
                    new Requirement
                    {
                        ClientId = clients[0].ClientId,
                        ProjectId = projects[0].ProjectId,
                        RequirementTitle = "User Authentication System",
                        Priority = "High",
                        RequirementDescription = "Implement secure user registration, login, and password reset functionality with email verification.",
                        Attachment = "auth_requirements.pdf",
                        IsActive = true,
                        IsNew = true,
                        Status = G3NexusBackend.Models.TaskStatus.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-55)
                    },
                    new Requirement
                    {
                        ClientId = clients[0].ClientId,
                        ProjectId = projects[0].ProjectId,
                        RequirementTitle = "Shopping Cart Functionality",
                        Priority = "High",
                        RequirementDescription = "Create shopping cart with add/remove items, quantity updates, and checkout process.",
                        Attachment = "cart_requirements.pdf",
                        IsActive = true,
                        IsNew = false,
                        Status = G3NexusBackend.Models.TaskStatus.Complete,
                        CreatedAt = DateTime.UtcNow.AddDays(-50)
                    },
                    new Requirement
                    {
                        ClientId = clients[1].ClientId,
                        ProjectId = projects[1].ProjectId,
                        RequirementTitle = "Biometric Authentication",
                        Priority = "High",
                        RequirementDescription = "Integrate fingerprint and face recognition for secure app access.",
                        Attachment = "biometric_specs.pdf",
                        IsActive = true,
                        IsNew = true,
                        Status = G3NexusBackend.Models.TaskStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-35)
                    },
                    new Requirement
                    {
                        ClientId = clients[2].ClientId,
                        ProjectId = projects[2].ProjectId,
                        RequirementTitle = "Lead Management",
                        Priority = "Medium",
                        RequirementDescription = "System to track leads from initial contact through conversion.",
                        Attachment = "lead_requirements.pdf",
                        IsActive = true,
                        IsNew = true,
                        Status = G3NexusBackend.Models.TaskStatus.UnderReview,
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    }
                };

                await context.Requirements!.AddRangeAsync(requirements);
                await context.SaveChangesAsync();

                // Seed Quotations
                var quotations = new List<Quotation>
                {
                    new Quotation
                    {
                        ClientId = clients[0].ClientId,
                        ProjectId = projects[0].ProjectId,
                        EmployeeId = employees[0].EmployeeId,
                        CreatedDate = DateTime.Now.AddDays(-55),
                        Type = Constants.FinalQuotationType,
                        TotalCost = 160000.00m
                    },
                    new Quotation
                    {
                        ClientId = clients[1].ClientId,
                        ProjectId = projects[1].ProjectId,
                        EmployeeId = employees[1].EmployeeId,
                        CreatedDate = DateTime.Now.AddDays(-40),
                        Type = Constants.AdvancedQuotationType,
                        TotalCost = 85000.00m
                    },
                    new Quotation
                    {
                        ClientId = clients[2].ClientId,
                        ProjectId = projects[2].ProjectId,
                        EmployeeId = employees[2].EmployeeId,
                        CreatedDate = DateTime.Now.AddDays(-25),
                        Type = Constants.RequirementQuotationType,
                        TotalCost = 50000.00m
                    }
                };

                await context.Quotations!.AddRangeAsync(quotations);
                await context.SaveChangesAsync();

                // Seed Quotation Requirements
                var quotationRequirements = new List<QuotationRequirement>
                {
                    new QuotationRequirement { QuotationId = quotations[0].QuotationId, RequirementId = requirements[0].RequirementId, RequirementCost = 50000.00m },
                    new QuotationRequirement { QuotationId = quotations[0].QuotationId, RequirementId = requirements[1].RequirementId, RequirementCost = 110000.00m },
                    new QuotationRequirement { QuotationId = quotations[1].QuotationId, RequirementId = requirements[2].RequirementId, RequirementCost = 85000.00m },
                    new QuotationRequirement { QuotationId = quotations[2].QuotationId, RequirementId = requirements[3].RequirementId, RequirementCost = 50000.00m }
                };

                await context.QuotationRequirements!.AddRangeAsync(quotationRequirements);
                await context.SaveChangesAsync();

                // Seed Bugs
                var bugs = new List<Bug>
                {
                    new Bug
                    {
                        ClientId = clients[0].ClientId,
                        ProjectId = projects[0].ProjectId,
                        BugTitle = "Login page not responsive on mobile",
                        Severity = "Medium",
                        BugDescription = "The login page layout breaks on mobile devices with screen width less than 768px.",
                        Attachment = "mobile_login_bug.png",
                        IsNew = true,
                        IsActive = true,
                        Status = G3NexusBackend.Models.TaskStatus.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Bug
                    {
                        ClientId = clients[0].ClientId,
                        ProjectId = projects[0].ProjectId,
                        BugTitle = "Payment gateway timeout error",
                        Severity = "High",
                        BugDescription = "Payment processing fails with timeout error after 30 seconds.",
                        Attachment = "payment_error_log.txt",
                        IsNew = false,
                        IsActive = true,
                        Status = G3NexusBackend.Models.TaskStatus.UnderReview,
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Bug
                    {
                        ClientId = clients[1].ClientId,
                        ProjectId = projects[1].ProjectId,
                        BugTitle = "Biometric authentication fails on some devices",
                        Severity = "High",
                        BugDescription = "Fingerprint authentication not working on older Android devices.",
                        Attachment = "biometric_compatibility.xlsx",
                        IsNew = true,
                        IsActive = true,
                        Status = G3NexusBackend.Models.TaskStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    }
                };

                await context.Bugs!.AddRangeAsync(bugs);
                await context.SaveChangesAsync();

                // Seed Payments
                var payments = new List<Payment>
                {
                    new Payment
                    {
                        ProjectId = projects[0].ProjectId,
                        ClientId = clients[0].ClientId,
                        PaymentAmount = 48000.00m,
                        PaymentType = "Initial Payment",
                        PaymentDescription = "30% initial payment for E-Commerce Platform project",
                        PaymentDate = DateTime.Now.AddDays(-45),
                        Attachment = "initial_payment_receipt.pdf"
                    },
                    new Payment
                    {
                        ProjectId = projects[0].ProjectId,
                        ClientId = clients[0].ClientId,
                        PaymentAmount = 64000.00m,
                        PaymentType = "Milestone Payment",
                        PaymentDescription = "40% payment after completing authentication module",
                        PaymentDate = DateTime.Now.AddDays(-20),
                        Attachment = "milestone1_receipt.pdf"
                    },
                    new Payment
                    {
                        ProjectId = projects[1].ProjectId,
                        ClientId = clients[1].ClientId,
                        PaymentAmount = 25500.00m,
                        PaymentType = "Initial Payment",
                        PaymentDescription = "30% initial payment for Mobile Banking App",
                        PaymentDate = DateTime.Now.AddDays(-35),
                        Attachment = "banking_app_initial.pdf"
                    }
                };

                await context.Payments!.AddRangeAsync(payments);
                await context.SaveChangesAsync();

                // Seed Verifications
                var verifications = new List<Verification>
                {
                    new Verification
                    {
                        Email = clients[0].Email,
                        VerificationCode = "123456",
                        ExpiryDate = DateTime.Now.AddHours(24)
                    },
                    new Verification
                    {
                        Email = clients[1].Email,
                        VerificationCode = "789012",
                        ExpiryDate = DateTime.Now.AddHours(24)
                    }
                };

                await context.Verifications!.AddRangeAsync(verifications);
                await context.SaveChangesAsync();

                // Seed Refresh Tokens
                var refreshTokens = new List<RefreshToken>
                {
                    new RefreshToken
                    {
                        Email = clients[0].Email,
                        Token = Guid.NewGuid().ToString(),
                        ExpiryDate = DateTime.Now.AddDays(7),
                        IsRevoked = false,
                        IsActive = true
                    },
                    new RefreshToken
                    {
                        Email = employees[0].Email,
                        Token = Guid.NewGuid().ToString(),
                        ExpiryDate = DateTime.Now.AddDays(7),
                        IsRevoked = false,
                        IsActive = true
                    }
                };

                await context.RefreshTokens!.AddRangeAsync(refreshTokens);
                await context.SaveChangesAsync();

                Console.WriteLine("Database seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                throw;
            }
        }

        private static async Task ClearExistingData(G3NexusDbContext context)
        {
            // Clear data in reverse order of dependencies
            context.RefreshTokens!.RemoveRange(context.RefreshTokens);
            context.Verifications!.RemoveRange(context.Verifications);
            context.Payments!.RemoveRange(context.Payments);
            context.Bugs!.RemoveRange(context.Bugs);
            context.QuotationRequirements!.RemoveRange(context.QuotationRequirements);
            context.Quotations!.RemoveRange(context.Quotations);
            context.Requirements!.RemoveRange(context.Requirements);
            context.QuotationCosts!.RemoveRange(context.QuotationCosts);
            context.ProjectTermsConditions!.RemoveRange(context.ProjectTermsConditions);
            context.EmployeeProjects!.RemoveRange(context.EmployeeProjects);
            context.Projects!.RemoveRange(context.Projects);
            context.TermsConditions!.RemoveRange(context.TermsConditions);
            context.Employees!.RemoveRange(context.Employees);
            context.Clients!.RemoveRange(context.Clients);
            context.Companies!.RemoveRange(context.Companies);
            
            await context.SaveChangesAsync();
            Console.WriteLine("Existing data cleared from database.");
        }
    }
}
