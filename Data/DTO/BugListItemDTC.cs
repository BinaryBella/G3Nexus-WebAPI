using System;

namespace G3NexusBackend.Data.DTO;

public class BugListItemDTC
{
    public int BugId { get; set; }
    public string BugTitle { get; set; }
    public string Severity { get; set; }
    public bool IsNew { get; set; }
    public int ClientId { get; set; }
    public int ProjectId { get; set; }
    public string ClientName { get; set; }
    public string ProjectName { get; set; }
}
