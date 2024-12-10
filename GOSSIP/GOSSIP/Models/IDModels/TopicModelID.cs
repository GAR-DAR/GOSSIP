using System.Security.RightsManagement;

namespace GOSSIP
{
    //Тимчасова модель постів. Пізніше буде замінена на робочу 
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

        public TopicModelID() { }

        public TopicModelID(uint iD, uint authorID, string title, string content,
        DateTime createdAt, int rating, List<string> tags, List<uint> repliesID,
        uint repliesCount, bool isDeleted)
        {
            ID = iD;
            AuthorID = authorID;
            Title = title;
            Content = content;
            CreatedAt = createdAt;
            Rating = rating;
            Tags = tags;
            Replies = repliesID;
            RepliesCount = repliesCount;
            IsDeleted = isDeleted;
        }

        public TopicModelID(TopicModel topicModel)
        {
            ID = topicModel.ID;
            AuthorID = topicModel.Author.ID;
            Title = topicModel.Title;
            Content = topicModel.Content;
            CreatedAt = topicModel.CreatedAt;
            Rating = topicModel.Rating;
            Tags = topicModel.Tags;
            foreach (var reply in topicModel.Replies)
            {
                Replies.Add(reply.ID);
            }
            RepliesCount = topicModel.RepliesCount;
            IsDeleted = topicModel.IsDeleted;
        }
    }
}