namespace G3NexusBackend.Data.DTO
{
    public class RequirementsByProjectRequestDTO
    {
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public bool IncludeQuoted { get; set; } = false; // Option to include already quoted requirements
    }

    public class BulkQuotationValidationDTO
    {
        public List<int> RequirementIds { get; set; } = new();
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
    }
}
