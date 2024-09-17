namespace G3NexusBackend.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
