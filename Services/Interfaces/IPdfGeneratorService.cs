using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IPdfGeneratorService
{
    byte[] GenerateProjectQuotation(ProjectDTO project);
}