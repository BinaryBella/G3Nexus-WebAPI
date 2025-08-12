namespace G3NexusBackend.Data.DTO
{
    public class RequirementListItemDTO
    {
        public int RequirementId { get; set; }
        public string? RequirementTitle { get; set; }
        public string? Priority { get; set; }
        public bool IsNew { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public string? ClientName { get; set; }
        public string? ProjectName { get; set; }
        public bool IsQuoted { get; set; }
        public bool IsSelected { get; set; } = false; // For UI selection purposes
    }
}