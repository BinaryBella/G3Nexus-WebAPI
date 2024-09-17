namespace G3NexusBackend.Models
{
    public class Requirement
    {
        public int RequirementId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string RequirementTitle { get; set; }
        public string Priority { get; set; }
        public string RequirementDescription { get; set; }
        public string Attachment { get; set; }
    }
}
