namespace G3NexusBackend.DTOs
{
    public class BugDTO
    {
        public int BugId { get; set; }
        public string BugTitle { get; set; }
        public string Severity { get; set; }
        public string BugDescription { get; set; }
        public string Attachment { get; set; }
        public bool IsActive { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
    }
}