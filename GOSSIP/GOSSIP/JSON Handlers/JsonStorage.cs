using GOSSIP.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace GOSSIP.JsonHandlers
{
    public class JsonStorage
    {
        private readonly string _filePath;
        private JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };

        public JsonStorage(string filePath)
        {
            _filePath = filePath;
        }

        public List<UserModel> LoadUsers()
        {
            if (!File.Exists(_filePath))
                return new List<UserModel>();

            string jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<UserModel>>(jsonData, options) ?? new List<UserModel>();
        }

        public List<TopicModel> LoadTopics()
        {
            if (!File.Exists(_filePath))
                return new List<TopicModel>();

            string jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TopicModel>>(jsonData, options) ?? new List<TopicModel>();
        }

        public void SaveUsers(List<UserModel> users)
        {
            string jsonData = JsonSerializer.Serialize(users, options);
            File.WriteAllText(_filePath, jsonData);
        }

        public void SaveTopic(TopicModel updatedTopic)
        {
            var allTopics = LoadTopics(); // Завантажуємо всю колекцію тем
            var index = allTopics.FindIndex(t => t.ID == updatedTopic.ID); // Знаходимо тему по ID

            if (index >= 0)
            {
                // Оновлюємо знайдений об'єкт
                allTopics[index] = updatedTopic;
            }
            else
            {
                allTopics.Add(updatedTopic); // Якщо не знайшли, додаємо нову
            }

            string json = JsonSerializer.Serialize(allTopics, options); // Перетворюємо колекцію на JSON
            File.WriteAllText(_filePath, json); // Перезаписуємо файл
        }

        public void SaveTopics(List<TopicModel> topics)
        {
            string json = JsonSerializer.Serialize(topics, options);
            File.WriteAllText(_filePath, json);
        }
    }

}
