namespace G3NexusBackend.DTOs
{
    public class ProjectDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string ProjectSize { get; set; }
        public DateTime CreationDate { get; set; }
        public string ProjectDescription { get; set; }
        public decimal EstimatedBudget { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalBudget { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}