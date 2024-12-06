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

    public static bool Delete(MessageModel message, MySqlConnection conn)
    {
        string deleteQuery =
           """
            UPDATE messages
            SET is_deleted = TRUE
            WHERE id = @message_id
            """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@message_id", message.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool Read(MessageModel message, MySqlConnection conn)
    {
        string readQuery =
           """
            UPDATE messages
            SET is_read = TRUE
            WHERE id = @message_id
            """;

        using var updateCommand = new MySqlCommand(readQuery, conn);
        updateCommand.Parameters.AddWithValue("@message_id", message.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
}