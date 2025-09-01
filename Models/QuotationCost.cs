namespace G3NexusBackend.Models;

public class QuotationCost
{
    public int Id { get; set; }
    public int ProjectId { get; set; }

    public decimal AdvancePayment { get; set; }
    public decimal DevelopmentCost { get; set; }
    public decimal HostingAndDomain { get; set; }
    public decimal SSLCertificate { get; set; }
    public decimal ServerCost { get; set; }

    public decimal DeploymentCost { get; set; }

    public Project Project { get; set; }
}