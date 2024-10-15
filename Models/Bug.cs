// Models/Bug.cs
public class Bug
{
    public int BugId { get; set; }
    public string BugTitle { get; set; }
    public string Severity { get; set; }
    public string BugDescription { get; set; }
    public string Attachment { get; set; }
    public bool IsActive { get; set; }
    
    // Foreign Keys
    public int ClientId { get; set; }
    public int ProjectId { get; set; }

    // Navigation Properties
    public Client Client { get; set; }
    public Project Project { get; set; }
}