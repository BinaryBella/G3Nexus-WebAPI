using G3NexusBackend.DTOs;
using G3NexusBackend.Models;

public interface IAuthService
{
    User? IsAuthenticated(string emailaddress, string password);
}