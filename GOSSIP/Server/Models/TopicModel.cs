namespace Server.Models
{
    public class TopicModel
    {
        public uint ID { get; set; }
        public UserModel Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public List<string> Tags { get; set; }
        public List<ParentReplyModel> Replies { get; set; }
        public uint RepliesCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}