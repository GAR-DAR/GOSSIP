using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TopicService
{
    private readonly JsonStorage _jsonStorage;
    private readonly List<TopicModel> _topics;

    public TopicService(string jsonFilePath)
    {
        _jsonStorage = new JsonStorage(jsonFilePath);
        _topics = _jsonStorage.LoadTopics();
    }

    // Додати новий пост
    public void AddTopic(TopicModel topic)
    {
        List<TopicModel> topics = _jsonStorage.LoadTopics();
        topic.ID = (uint)topics.Count + 1;
        _topics.Add(topic);
        _jsonStorage.SaveTopics(_topics);
    }

    public void AddReply(uint topicId, ParentReplyModel reply)
    {
        var topic = _topics.FirstOrDefault(t => t.ID == topicId);
        if (topic == null) throw new Exception("Topic not found");

        // TODO: uncomment this line, after DB will be ready
        //topic.Replies.Add(reply);
        topic.RepliesCount++;

        _jsonStorage.SaveTopics(_topics);
    }

    // Отримати список усіх постів
    public List<TopicModel> GetAllTopics()
    {
        return _topics;
    }

    // Знайти пост за ID
    public TopicModel? GetTopicById(uint topicId)
    {
        return _topics.FirstOrDefault(t => t.ID == topicId);
    }

    public List<TopicModel> GetTopicsByID(uint id)
    {
        return _topics.Where((TopicModel t) => t.Author.ID == id).ToList();
    }
}
