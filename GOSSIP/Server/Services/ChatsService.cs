using System.Data;
using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

public static class ChatsService
{
    public static bool Create(string name, List<uint> userIds, MySqlConnection conn)
    {
        string createQuery =
            """
            INSERT INTO chats (created_at, name, is_deleted) 
            VALUES (NOW(), @name, FALSE) 
            """;

        using var insertCommand = new MySqlCommand(createQuery, conn);
        insertCommand.Parameters.AddWithValue("@name", name);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false; // TODO: an exception, maybe?..

        foreach (var userId in userIds)
        {
            string addUserQuery =
                """
                INSERT INTO chats_to_users (chat_id, user_id)
                VALUES (@chat_id, @user_id)
                """;

            using var addUserCommand = new MySqlCommand(addUserQuery, conn);
            addUserCommand.Parameters.AddWithValue("@chat_id", insertCommand.LastInsertedId);
            addUserCommand.Parameters.AddWithValue("@user_id", userId);

            addUserCommand.ExecuteNonQuery();
        }

        return true;
    }

    public static bool AddUser(uint chatId, uint userId, MySqlConnection conn)
    {
        string addUserQuery =
            """
            INSERT INTO chats_to_users (chat_id, user_id)
            VALUES (@chat_id, @user_id)
            """;

        using var insertCommand = new MySqlCommand(addUserQuery, conn);
        insertCommand.Parameters.AddWithValue("@chat_id", chatId);
        insertCommand.Parameters.AddWithValue("@user_id", userId);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool DeleteUser(uint chatId, uint userId, MySqlConnection conn)
    {
        string deleteUserQuery =
            """
            DELETE FROM chats_to_users WHERE user_id = @user_id AND chat_id = @chat_id
            """;

        using var deleteCommand = new MySqlCommand(deleteUserQuery, conn);
        deleteCommand.Parameters.AddWithValue("@user_id", userId);
        deleteCommand.Parameters.AddWithValue("@chat_id", chatId);

        int rowsAffected = deleteCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static List<uint> SelectChatIdsByUser(uint userId, MySqlConnection conn)
    {
        List<uint> chatIds = [];
        
        string selectByUserQuery =
            """
            SELECT chats.id
            FROM chats
            LEFT JOIN chats_to_users ON chats.id = chats_to_users.chat_id
            LEFT JOIN users ON chats_to_users.user_id = users.id
            WHERE chats.is_deleted = FALSE
            AND users.id = @user_id
            """;

        using var selectCommand = new MySqlCommand(selectByUserQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", userId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            chatIds.Add(reader.GetUInt32("id"));
        }

        return chatIds;
    }

    public static bool Delete(uint id, MySqlConnection conn)
    {
        string deleteQuery =
            $"""
             UPDATE chats
             SET is_deleted = TRUE
             WHERE id = @id
             """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", id);

        int affectedRows = updateCommand.ExecuteNonQuery();
        return affectedRows != 0;
    }

    public static List<uint> SelectUserIdsByChat(uint id, MySqlConnection conn)
    {
        List<uint> userIds = [];

        string selectUsersByIdQuery =
            """
            SELECT users.id
            FROM users
            LEFT JOIN chats_to_users ON users.id = chats_to_users.user_id
            WHERE chats_to_users.chat_id = @chat_id 
            """;
        
        using var selectUsersCommand = new MySqlCommand(selectUsersByIdQuery, conn);
        selectUsersCommand.Parameters.AddWithValue("@chat_id", id);

        using var usersReader = selectUsersCommand.ExecuteReader();
        while (usersReader.Read())
        {
            userIds.Add(usersReader.GetUInt32("id"));
        }

        return userIds;
    }

    public static ChatModelID SelectById(uint id, MySqlConnection conn)
    {
        var chat = new ChatModelID();
        string selectByIdQuery =
            """
            SELECT name, created_at, is_deleted
            FROM chats
            WHERE id = @chat_id
            """;

        using var selectCommand = new MySqlCommand(selectByIdQuery, conn);
        selectCommand.Parameters.AddWithValue("@chat_id", id);

        using var chatReader = selectCommand.ExecuteReader();
        while (chatReader.Read())
        {
            chat.ID = id;
            chat.Name = chatReader.GetString("name");
            chat.CreatedAt = chatReader.GetDateTime("created_at");
            chat.IsDeleted = chatReader.GetBoolean("is_deleted");
        }
        
        chatReader.Close();

        chat.UserIDs = SelectUserIdsByChat(id, conn);
        chat.MessageIDs = SelectMessageIdsByChat(id, conn);

        return chat;
    }

    public static List<uint> SelectMessageIdsByChat(uint id, MySqlConnection conn)
    {
        List<uint> messageIds = [];
        string selectMessageIdsQuery =
            """
            SELECT id
            FROM messages
            WHERE chat_id = @chat_id
            AND is_deleted = FALSE
            """;

        using var selectCommand = new MySqlCommand(selectMessageIdsQuery, conn);
        selectCommand.Parameters.AddWithValue("@chat_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            messageIds.Add(reader.GetUInt32("id"));
        }

        return messageIds;
    }
}