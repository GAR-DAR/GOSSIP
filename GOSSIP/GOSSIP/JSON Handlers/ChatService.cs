using GOSSIP.JsonHandlers;
using GOSSIP;
using System.Collections.ObjectModel;

public class ChatService
{
    private readonly JsonStorage _jsonStorage;
    private readonly List<UserModelID> _users;

    public ChatService(string jsonFilePath)
    {
        _jsonStorage = new JsonStorage(jsonFilePath);
        _users = _jsonStorage.LoadUsers();
    }

    // Додати нове повідомлення
    public void AddMessage(uint chatId, MessageModelID message)
    {
        var user = _users.FirstOrDefault(u => u.ChatsID.Any(c => c.ID == chatId));
        if (user == null) throw new Exception("User or chat not found");

        var chat = user.Chats.First(c => c.ID == chatId);
        chat.Messages.Add(message);

        // Зберегти всі зміни
        _jsonStorage.SaveUsers(_users);
    }

    // Додати нового користувача
    public void AddUser(UserModelID user)
    {
        _users.Add(user);
        _jsonStorage.SaveUsers(_users);
    }

    public void AddChat(uint userId, ChatModelID chat)
    {
        // Знайти користувача за ID
        var user = _users.FirstOrDefault(u => u.ID == userId);
        if (user == null)
            throw new Exception("User not found");

        // Перевірити, чи чат із таким ID уже існує у користувача
        if (user.ChatsID.Any(c => c.ID == chat.ID))
            throw new Exception("Chat with this ID already exists for the user");

        // Додати новий чат
        user.ChatsID.Add(chat);

        // Зберегти всі зміни
        _jsonStorage.SaveUsers(_users);
    }
}

