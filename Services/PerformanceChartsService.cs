using G3NexusBackend.Data;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace G3NexusBackend.Services
{
    public class PerformanceChartsService : IPerformanceChartsService
    {
        private readonly G3NexusDbContext _context;

        public PerformanceChartsService(G3NexusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ProjectOverviewDto> GetProjectOverviewDataAsync(int monthsBack = 6)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-monthsBack);

            // Get monthly data for the specified period
            var monthlyData = new List<ProjectStatsDto>();
            
            for (int i = monthsBack - 1; i >= 0; i--)
            {
                var monthStart = endDate.AddMonths(-i).Date.AddDays(1 - endDate.AddMonths(-i).Day);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var completedProjects = await _context.Projects
                    .Where(p => p.ActualEndDate.HasValue && 
                               p.ActualEndDate.Value >= monthStart && 
                               p.ActualEndDate.Value <= monthEnd &&
                               p.Status.ToLower() == "completed")
                    .CountAsync();

                var activeProjects = await _context.Projects
                    .Where(p => p.IsActive && 
                               p.ActualStartDate.HasValue &&
                               p.ActualStartDate.Value <= monthEnd &&
                               (!p.ActualEndDate.HasValue || p.ActualEndDate.Value >= monthStart) &&
                               p.Status.ToLower() != "completed")
                    .CountAsync();

                monthlyData.Add(new ProjectStatsDto
                {
                    CompletedProjects = completedProjects,
                    ActiveProjects = activeProjects,
                    Period = monthStart
                });
            }

            return new ProjectOverviewDto
            {
                Labels = monthlyData.Select(m => m.Period.ToString("MMM", CultureInfo.InvariantCulture)).ToArray(),
                Datasets = new[]
                {
                    new ProjectDatasetDto
                    {
                        Label = "Completed Projects",
                        Data = monthlyData.Select(m => m.CompletedProjects).ToArray(),
                        BackgroundColor = "rgba(52, 80, 163, 0.8)",
                        BorderColor = "rgba(52, 80, 163, 1)",
                        BorderWidth = 2
                    },
                    new ProjectDatasetDto
                    {
                        Label = "Active Projects",
                        Data = monthlyData.Select(m => m.ActiveProjects).ToArray(),
                        BackgroundColor = "rgba(255, 191, 0, 0.8)",
                        BorderColor = "rgba(255, 191, 0, 1)",
                        BorderWidth = 2
                    }
                }
            };
        }

        public async Task<RequirementsStatusDto> GetRequirementsStatusDataAsync()
        {
            // Since the Requirement model doesn't have a specific status field,
            // we'll determine status based on available fields
            var allRequirements = await _context.Requirements
                .Where(r => r.IsActive)
                .ToListAsync();

            // Define status logic based on your business rules
            // You may need to adjust this based on how you determine requirement status
            var pending = allRequirements.Count(r => r.IsNew && !r.IsQuoted);
            var inProgress = allRequirements.Count(r => !r.IsNew && !r.IsQuoted);
            var underReview = allRequirements.Count(r => r.IsQuoted && r.QuotationId.HasValue);
            var completed = allRequirements.Count(r => r.IsQuoted && r.Project != null && 
                                                      r.Project.Status.ToLower() == "completed");

            return new RequirementsStatusDto
            {
                Labels = new[] { "Pending", "In Progress", "Under Review", "Completed" },
                Datasets = new[]
                {
                    new RequirementDatasetDto
                    {
                        Data = new[] { pending, inProgress, underReview, completed },
                        BackgroundColor = new[]
                        {
                            "rgba(239, 68, 68, 0.8)",
                            "rgba(245, 158, 11, 0.8)",
                            "rgba(59, 130, 246, 0.8)",
                            "rgba(34, 197, 94, 0.8)"
                        },
                        BorderColor = new[]
                        {
                            "rgba(239, 68, 68, 1)",
                            "rgba(245, 158, 11, 1)",
                            "rgba(59, 130, 246, 1)",
                            "rgba(34, 197, 94, 1)"
                        },
                        BorderWidth = 2
                    }
                }
            };
        }

        public async Task<BugTrendDto> GetBugTrendDataAsync(int weeksBack = 6)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-weeksBack * 7);

            var weeklyData = new List<BugStatsDto>();

            for (int i = weeksBack - 1; i >= 0; i--)
            {
                var weekStart = endDate.AddDays(-((i + 1) * 7)).Date;
                var weekEnd = weekStart.AddDays(6);

                var bugsReported = await _context.Bugs
                    .Where(b => b.CreatedAt >= weekStart && b.CreatedAt <= weekEnd)
                    .CountAsync();

                // Assuming bugs are resolved when they are quoted and associated with completed projects
                // You may need to adjust this logic based on your business rules
                var bugsResolved = await _context.Bugs
                    .Where(b => b.IsQuoted && 
                               b.QuotationId.HasValue &&
                               b.Project != null &&
                               b.Project.ActualEndDate.HasValue &&
                               b.Project.ActualEndDate.Value >= weekStart &&
                               b.Project.ActualEndDate.Value <= weekEnd)
                    .CountAsync();

                weeklyData.Add(new BugStatsDto
                {
                    BugsReported = bugsReported,
                    BugsResolved = bugsResolved,
                    WeekStart = weekStart,
                    WeekEnd = weekEnd
                });
            }

            return new BugTrendDto
            {
                Labels = weeklyData.Select((_, index) => $"Week {index + 1}").ToArray(),
                Datasets = new[]
                {
                    new BugDatasetDto
                    {
                        Label = "Bugs Reported",
                        Data = weeklyData.Select(w => w.BugsReported).ToArray(),
                        Fill = true,
                        BackgroundColor = "rgba(239, 68, 68, 0.1)",
                        BorderColor = "rgba(239, 68, 68, 1)",
                        PointBackgroundColor = "rgba(239, 68, 68, 1)",
                        PointBorderColor = "#fff",
                        PointBorderWidth = 2,
                        PointRadius = 6,
                        Tension = 0.4
                    },
                    new BugDatasetDto
                    {
                        Label = "Bugs Resolved",
                        Data = weeklyData.Select(w => w.BugsResolved).ToArray(),
                        Fill = true,
                        BackgroundColor = "rgba(34, 197, 94, 0.1)",
                        BorderColor = "rgba(34, 197, 94, 1)",
                        PointBackgroundColor = "rgba(34, 197, 94, 1)",
                        PointBorderColor = "#fff",
                        PointBorderWidth = 2,
                        PointRadius = 6,
                        Tension = 0.4
                    }
                }
            };
        }

        public async Task<PerformanceChartsResponseDto> GetAllPerformanceDataAsync()
        {
            var projectsData = await GetProjectOverviewDataAsync();
            var requirementsData = await GetRequirementsStatusDataAsync();
            var bugsData = await GetBugTrendDataAsync();

            return new PerformanceChartsResponseDto
            {
                ProjectsData = projectsData,
                RequirementsData = requirementsData,
                BugsData = bugsData
            };
        }
    }
}
