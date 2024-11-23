using System.Security.RightsManagement;

namespace GOSSIP.Models
{
    //Тимчасова модель постів. Пізніше буде замінена на робочу 
    public class Post (User author, DateTime createdAt, string title, string content,
        List<string> tags, int repliesCount, int rating)
    {
        public User Author { get; set; } = author;
        public string Title { get; set; } = title;
        public string Content { get; set; } = content;
        public DateTime CreatedAt { get; set; } = createdAt;
        public int Rating { get; set; } = rating;
        public List<string> Tags { get; set; } = tags;
        public int RepliesCount { get; set; } = repliesCount;
    }
}
