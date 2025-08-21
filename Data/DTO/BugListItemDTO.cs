using System;
using G3NexusBackend.Models;

namespace G3NexusBackend.Data.DTO;

public class BugListItemDTO
{
    public int BugId { get; set; }
    public string? BugTitle { get; set; }
    public string? Severity { get; set; }
    public bool IsNew { get; set; }
    public G3NexusBackend.Models.TaskStatus Status { get; set; }
    public int ClientId { get; set; }
    public int ProjectId { get; set; }
    public string? ClientName { get; set; }
    public string? ProjectName { get; set; }
    public bool IsQuoted { get; set; }
    public bool IsSelected { get; set; } = false; // For UI selection purposes
}
