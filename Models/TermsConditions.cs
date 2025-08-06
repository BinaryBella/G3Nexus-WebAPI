namespace G3NexusBackend.Models;

public class TermsConditions
{
    public int TCId { get; set; }
    public string Content { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public ICollection<ProjectTermsConditions> ProjectTermsConditions { get; set; }
}