using System;
using System.Collections.Generic;

namespace G3NexusBackend.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public decimal TotalCost { get; set; }

        public Employee Employee { get; set; }
        public Client Client { get; set; }
        public Project Project { get; set; }


    }
}