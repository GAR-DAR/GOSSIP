namespace GOSSIP.Models
{
    public class Post (User author, DateTime createdAt, string title, string content)
    {
        public User Author { get; set; } = author;
        public string Title { get; set; } = title;
        public string Content { get; set; } = content;
        public DateTime CreatedAt { get; set; } = createdAt;

    }
}
