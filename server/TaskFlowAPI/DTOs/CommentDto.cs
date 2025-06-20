namespace TaskFlowAPI.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
