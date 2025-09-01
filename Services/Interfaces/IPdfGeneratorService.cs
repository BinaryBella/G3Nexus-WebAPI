using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;

namespace G3NexusBackend.Services.Interfaces;

public interface IPdfGeneratorService
{
    byte[] GenerateProjectQuotation(ProjectResponseDTO project, string clientName, string clientContact, string clientEmail, List<string> selectedTerms);

    byte[] GenerateBugQuotation(Bug bug, BugQuotationRequestDTO quotationRequest, string clientContact,
        string clientEmail, string projectName);
    byte[] GenerateRequirementQuotation(Requirement requirement, RequirementQuotationRequestDTO quotationRequest, string clientContact, string clientEmail, string projectName);
}