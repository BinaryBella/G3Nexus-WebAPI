using G3NexusBackend.Data;

namespace G3NexusBackend.Extensions
{
    public static class ServiceExtensions
    {
        public static async Task<IHost> SeedDatabaseAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<G3NexusDbContext>();
                await DatabaseSeeder.SeedDataAsync(context);
            }
            return host;
        }
    }
}
