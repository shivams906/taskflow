namespace TaskFlowAPI.DTOs
{
    public record CreateWorkspaceDto
    {
        public required string Name { get; set; }
    }
}
