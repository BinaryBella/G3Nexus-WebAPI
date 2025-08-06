namespace G3NexusBackend.Models;

public class ProjectTermsConditions
{
    public int ProjectId { get; set; }
    public Project Project { get; set; }

    public int TCId { get; set; }
    public TermsConditions TermsConditions { get; set; }

    public bool IsChecked { get; set; } // User selected or not
}