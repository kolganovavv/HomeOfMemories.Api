namespace HomeOfMemories.Api.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int MemoryId { get; set; }
        public Memory? Memory { get; set; }
        public string Path { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
