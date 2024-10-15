// Models/Requirement.cs
public class Requirement
{
    public int RequirementId { get; set; }
    public string RequirementTitle { get; set; }
    public string Priority { get; set; }
    public string RequirementDescription { get; set; }
    public string Attachment { get; set; }
    public bool IsActive { get; set; }
    // Foreign Keys
    public int ClientId { get; set; }
    public int ProjectId { get; set; }

    // Navigation Properties
    public Client Client { get; set; }
    public Project Project { get; set; }
}