namespace G3NexusBackend.Models;

public class ClientProject
{
    public int ClientId { get; set; }
    public Client Client { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; }
}