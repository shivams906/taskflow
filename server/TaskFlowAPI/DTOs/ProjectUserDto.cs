namespace TaskFlowAPI.DTOs
{
    public record ProjectUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
