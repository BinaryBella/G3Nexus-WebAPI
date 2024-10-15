using G3NexusBackend.DTOs;

public class ProjectEmployeeClientDTO
{
    public int ProjectId { get; set; }
    public int EmployeeId { get; set; }
    public int ClientId { get; set; }

    public ProjectDTO Project { get; set; } // Embedded ProjectDTO
    public EmployeeDTO Employee { get; set; } // Embedded EmployeeDTO
    public ClientDTO Client { get; set; } // Embedded ClientDTO
}