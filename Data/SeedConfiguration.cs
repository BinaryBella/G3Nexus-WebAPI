namespace G3NexusBackend.Data
{
    public static class SeedConfiguration
    {
        public static bool SeedDatabaseOnStartup { get; set; } = true;
        public static bool RecreateDatabase { get; set; } = false;
        public static bool SeedTestData { get; set; } = true;
        
        // You can add more configuration options here
        public static int NumberOfTestCompanies { get; set; } = 3;
        public static int NumberOfTestClientsPerCompany { get; set; } = 1;
        public static int NumberOfTestEmployees { get; set; } = 4;
        public static int NumberOfTestProjects { get; set; } = 3;
    }
}
