namespace G3NexusBackend.Data.DTO
{
    public class ProjectOverviewDto
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public ProjectDatasetDto[] Datasets { get; set; } = Array.Empty<ProjectDatasetDto>();
    }

    public class ProjectDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public int[] Data { get; set; } = Array.Empty<int>();
        public string BackgroundColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public int BorderWidth { get; set; } = 2;
    }

    public class RequirementsStatusDto
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public RequirementDatasetDto[] Datasets { get; set; } = Array.Empty<RequirementDatasetDto>();
    }

    public class RequirementDatasetDto
    {
        public int[] Data { get; set; } = Array.Empty<int>();
        public string[] BackgroundColor { get; set; } = Array.Empty<string>();
        public string[] BorderColor { get; set; } = Array.Empty<string>();
        public int BorderWidth { get; set; } = 2;
    }

    public class BugTrendDto
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public BugDatasetDto[] Datasets { get; set; } = Array.Empty<BugDatasetDto>();
    }

    public class BugDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public int[] Data { get; set; } = Array.Empty<int>();
        public bool Fill { get; set; } = true;
        public string BackgroundColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public string PointBackgroundColor { get; set; } = string.Empty;
        public string PointBorderColor { get; set; } = "#fff";
        public int PointBorderWidth { get; set; } = 2;
        public int PointRadius { get; set; } = 6;
        public double Tension { get; set; } = 0.4;
    }

    public class PerformanceChartsResponseDto
    {
        public ProjectOverviewDto ProjectsData { get; set; } = new();
        public RequirementsStatusDto RequirementsData { get; set; } = new();
        public BugTrendDto BugsData { get; set; } = new();
    }

    public class ProjectStatsDto
    {
        public int CompletedProjects { get; set; }
        public int ActiveProjects { get; set; }
        public DateTime Period { get; set; }
    }

    public class RequirementStatsDto
    {
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int UnderReview { get; set; }
        public int Completed { get; set; }
    }

    public class BugStatsDto
    {
        public int BugsReported { get; set; }
        public int BugsResolved { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
}
