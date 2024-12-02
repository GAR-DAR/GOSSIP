using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

public static class MessagesService
{
    public static bool Add(MessageModel message, MySqlConnection conn)
    {
        string addQuery =
            """
            INSERT INTO messages (chat_id, sender_id, content, sent_at, is_read, is_deleted) 
            VALUES (@chat_id, @sender_id, @content, @sent_at, FALSE, FALSE)
            """;

        using var insertCommand = new MySqlCommand(addQuery, conn);
        insertCommand.Parameters.AddWithValue("@chat_id", message.Chat.ID);
        insertCommand.Parameters.AddWithValue("@sender_id", message.User.ID);
        insertCommand.Parameters.AddWithValue("@content", message.MessageText);
        insertCommand.Parameters.AddWithValue("@sent_at", message.TimeStamp);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
}