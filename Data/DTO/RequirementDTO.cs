namespace G3NexusBackend.DTOs
{
    public class RequirementDTO
    {
        public int RequirementId { get; set; }
        public string RequirementTitle { get; set; }
        public string Priority { get; set; }
        public string RequirementDescription { get; set; }
        public string Attachment { get; set; }
        public bool IsActive { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
    }
}