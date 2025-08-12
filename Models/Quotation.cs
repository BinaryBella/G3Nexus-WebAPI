using System;
using System.Collections.Generic;

namespace G3NexusBackend.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; } // e.g., Draft, Sent
        public decimal TotalCost { get; set; }

        public ICollection<QuotationRequirement> QuotationRequirements { get; set; }
        public ICollection<QuotationBug> QuotationBugs { get; set; }
    }
}