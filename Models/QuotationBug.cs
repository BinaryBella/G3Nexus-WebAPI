namespace G3NexusBackend.Models;

public class QuotationBug
{
    public int QuotationBugId { get; set; }
    public int QuotationId { get; set; }
    public int BugId { get; set; }
    public decimal BugCost { get; set; }

    public Quotation? Quotation { get; set; }
    public Bug? Bug { get; set; }
}