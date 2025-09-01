namespace G3NexusBackend.Data.DTO;

public class AuthResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
}