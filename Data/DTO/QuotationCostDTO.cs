namespace G3NexusBackend.Data.DTO;

public class QuotationCostDTO
{
    public int ProjectId { get; set; }
    public decimal AdvancePayment { get; set; }
    public decimal DevelopmentCost { get; set; }
    public decimal HostingAndDomain { get; set; }
    public decimal SSLCertificate { get; set; }
    public decimal DeploymentCost { get; set; }
}