using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

// TODO: Test creating
// TODO: Delete a chat
// TODO: Retrieve chats
// TODO: Add single user
public static class ChatsService
{
    public static bool Create(ChatModel chat, MySqlConnection conn)
    {
        string createQuery =
            """
            INSERT INTO chats (created_at, name, is_deleted) 
            VALUES (@created_at, @name, FALSE) 
            """;

        using var insertCommand = new MySqlCommand(createQuery, conn);
        insertCommand.Parameters.AddWithValue("@created_at", chat.CreatedAt);
        insertCommand.Parameters.AddWithValue("@name", chat.Name);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false; // TODO: an exception, maybe?..

        foreach (var user in chat.Users)
        {
            string addUserQuery =
                """
                INSERT INTO chats_to_users (chat_id, user_id)
                VALUES (@chat_id, @user_id)
                """;

            using var addUserCommand = new MySqlCommand(addUserQuery, conn);
            addUserCommand.Parameters.AddWithValue("@chat_id", insertCommand.LastInsertedId);
            addUserCommand.Parameters.AddWithValue("@user_id", user.ID);

            addUserCommand.ExecuteNonQuery();
        }

        return true;
    }
}