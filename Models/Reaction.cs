namespace HomeOfMemories.Api.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public int MemoryId { get; set; }
        public Memory? Memory { get; set; }

        public string Emotion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
