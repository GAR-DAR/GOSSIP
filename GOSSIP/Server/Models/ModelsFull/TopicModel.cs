namespace Server
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

        public TopicModel(uint iD, UserModel author, string title, string content,
            DateTime createdAt, int rating, List<string> tags, List<ParentReplyModel> replies,
            uint repliesCount, bool isDeleted)
        {
            ID = iD;
            Author = author;
            Title = title;
            Content = content;
            CreatedAt = createdAt;
            Rating = rating;
            Tags = tags;
            Replies = replies;
            RepliesCount = repliesCount;
            IsDeleted = isDeleted;
        }

        public TopicModel() { }

        public TopicModel(TopicModelID topic)
        {
            ID = topic.ID;
            Title = topic.Title;
            Content = topic.Content;
            CreatedAt = topic.CreatedAt;
            Rating = topic.Rating;
            RepliesCount = topic.RepliesCount;
            IsDeleted = topic.IsDeleted;
            Author = null;
            Replies = null;
        }
    }
}