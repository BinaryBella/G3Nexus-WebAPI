using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IPdfGeneratorService
{
    byte[] GenerateProjectQuotation(ProjectResponseDTO project);
        byte[] GenerateProjectQuotation(ProjectResponseDTO project, string clientName, string clientContact, string clientEmail);
    }