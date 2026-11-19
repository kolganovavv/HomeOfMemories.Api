namespace HomeOfMemories.Api.Models
{
    public class Memory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        public string? YearLabel { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Photo> Photos { get; set; } = new();
        public List<MemoryTag> MemoryTags { get; set; } = new();
        public List<Reaction> Reactions { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}
