using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces
{
    public interface IPerformanceChartsService
    {
        Task<ProjectOverviewDto> GetProjectOverviewDataAsync(int monthsBack = 6);
        Task<RequirementsStatusDto> GetRequirementsStatusDataAsync();
        Task<BugTrendDto> GetBugTrendDataAsync(int weeksBack = 6);
        Task<PerformanceChartsResponseDto> GetAllPerformanceDataAsync();
    }
}
