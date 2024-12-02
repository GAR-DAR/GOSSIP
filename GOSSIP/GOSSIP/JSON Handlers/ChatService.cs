using GOSSIP.JsonHandlers;
using GOSSIP.Models;
using System.Collections.ObjectModel;

public class ChatService
{
    private readonly JsonStorage _jsonStorage;
    private readonly List<UserModel> _users;

    public ChatService(string jsonFilePath)
    {
        _jsonStorage = new JsonStorage(jsonFilePath);
        _users = _jsonStorage.LoadUsers();
    }

    // Додати нове повідомлення
    public void AddMessage(uint chatId, MessageModel message)
    {
        var user = _users.FirstOrDefault(u => u.Chats.Any(c => c.ID == chatId));
        if (user == null) throw new Exception("User or chat not found");

        var chat = user.Chats.First(c => c.ID == chatId);
        chat.Messages.Add(message);

        // Зберегти всі зміни
        _jsonStorage.SaveUsers(_users);
    }

    // Додати нового користувача
    public void AddUser(UserModel user)
    {
        _users.Add(user);
        _jsonStorage.SaveUsers(_users);
    }
}

