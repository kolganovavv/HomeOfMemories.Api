namespace HomeOfMemories.Api.Models
{
    public class Favorite
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public int MemoryId { get; set; }
        public Memory? Memory { get; set; }
    }
}
