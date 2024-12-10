namespace Server.Models
{
    public class TopicModelID
    {
        public uint ID { get; set; }
        public uint AuthorID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public List<string> Tags { get; set; }
        public List<uint> Replies { get; set; }
        public uint RepliesCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}