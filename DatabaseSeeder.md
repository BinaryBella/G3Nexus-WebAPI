# G3Nexus Database Seeder

This database seeder provides sample data for the G3Nexus backend application.

## Usage

### Automatic Seeding
The seeder runs automatically when the application starts if the database is empty. This is configured in `Program.cs`.

### Manual Seeding
You can also call the seeder manually:

```csharp
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<G3NexusDbContext>();
await DatabaseSeeder.SeedDataAsync(context, forceReseed: true);
```

## Seeded Data

The seeder creates the following test data:

### Companies (3)
- TechCorp Solutions
- Digital Innovations Ltd  
- Global Software Systems

### Clients (3)
- John Smith (TechCorp Solutions)
- Sarah Johnson (Digital Innovations Ltd)
- Michael Brown (Global Software Systems)

### Employees (4)
- Alice Developer (Developer)
- Bob Architect (Solution Architect)
- Carol Tester (QA Engineer)
- David Manager (Project Manager)

### Projects (3)
- E-Commerce Platform (Large, In Progress)
- Mobile Banking App (Medium, In Progress)
- CRM System (Small, Planning)

### Additional Data
- Terms & Conditions (2 sets)
- Requirements (4 total)
- Quotations (3 total)
- Quotation Costs
- Employee-Project assignments
- Bugs (3 sample bugs)
- Payments (3 sample payments)
- Verification codes
- Refresh tokens

## Configuration

You can configure seeding behavior in `Data/SeedConfiguration.cs`:

```csharp
SeedConfiguration.SeedDatabaseOnStartup = true;  // Enable/disable auto-seeding
SeedConfiguration.SeedTestData = true;           // Enable/disable test data
SeedConfiguration.NumberOfTestCompanies = 3;     // Number of test companies
```

## EF Core Commands

### Create Migration
```bash
dotnet ef migrations add InitialCreate
```

### Update Database
```bash
dotnet ef database update
```

### Remove Migration
```bash
dotnet ef migrations remove
```

### Drop Database
```bash
dotnet ef database drop
```

## Password Information

Test passwords for seeded accounts:
- Clients: `Password123!`
- Employees: `DevPassword123!`, `ArchPassword123!`, `TestPassword123!`, `MgrPassword123!`

**Note**: In production, implement proper password hashing using BCrypt or similar libraries.
