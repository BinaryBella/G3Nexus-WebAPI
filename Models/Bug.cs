namespace G3NexusBackend.Models
{
    public class Bug
    {
        public int BugId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string BugTitle { get; set; }
        public string Severity { get; set; }
        public string BugDescription { get; set; }
        public string Attachment { get; set; }
    }
}
