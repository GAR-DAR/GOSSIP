namespace Server.Models
{
    public class TopicModel (uint iD, uint userID, UserModel author, DateTime createdAt, string title, string content,
        List<string> tags, uint repliesCount, int rating, bool isDeleted)
    {
        public uint ID { get; set; } = iD;
        public UserModel Author { get; set; } = author;
        public string Title { get; set; } = title;
        public string Content { get; set; } = content;
        public DateTime CreatedAt { get; set; } = createdAt;
        public int Rating { get; set; } = rating;
        public List<string> Tags { get; set; } = tags;
        public List<ReplyModel> Replies { get; set; }
        public uint RepliesCount { get; set; } = repliesCount;
        public bool IsDeleted { get; set; } = isDeleted;
    }
}