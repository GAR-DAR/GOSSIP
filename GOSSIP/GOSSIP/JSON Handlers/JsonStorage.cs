using GOSSIP.Models;
using System.IO;
using System.Text.Json;

namespace GOSSIP.JsonHandlers
{
    public class JsonStorage
    {
        private readonly string _filePath;

        public JsonStorage(string filePath)
        {
            _filePath = filePath;
        }

        // Зчитати список користувачів із JSON
        public List<UserModel> LoadUsers()
        {
            if (!File.Exists(_filePath))
                return new List<UserModel>();

            string jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<UserModel>>(jsonData) ?? new List<UserModel>();
        }

        // Записати список користувачів у JSON
        public void SaveUsers(List<UserModel> users)
        {
            string jsonData = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, jsonData);
        }
    }

}
