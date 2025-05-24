namespace TaskFlowAPI.DTOs
{
    public record WorkspaceUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
