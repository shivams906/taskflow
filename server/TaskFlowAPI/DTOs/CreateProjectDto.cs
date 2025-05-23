namespace TaskFlowAPI.DTOs
{
    public record CreateProjectDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
