using MySql.Data.MySqlClient;

namespace Server.Services;

public static class MessagesService
{
    public static MessageModelID? Add(MessageModelID message, MySqlConnection conn)
    {
        string addQuery =
            """
            INSERT INTO messages (chat_id, sender_id, content, sent_at, is_read, is_deleted) 
            VALUES (@chat_id, @sender_id, @content, NOW(), FALSE, FALSE)
            """;

        using var insertCommand = new MySqlCommand(addQuery, conn);
        insertCommand.Parameters.AddWithValue("@chat_id", message.ChatID);
        insertCommand.Parameters.AddWithValue("@sender_id", message.UserID);
        insertCommand.Parameters.AddWithValue("@content", message.MessageText);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return null;

        message.ID = (uint)insertCommand.LastInsertedId;
        return message;
    }

    public static bool Delete(uint id, MySqlConnection conn)
    {
        string deleteQuery =
           """
            UPDATE messages
            SET is_deleted = TRUE
            WHERE id = @message_id
            """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@message_id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool Read(uint id, MySqlConnection conn)
    {
        string readQuery =
           """
            UPDATE messages
            SET is_read = TRUE
            WHERE id = @message_id
            """;

        using var updateCommand = new MySqlCommand(readQuery, conn);
        updateCommand.Parameters.AddWithValue("@message_id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static MessageModelID SelectById(uint id, MySqlConnection conn)
    {
        var message = new MessageModelID();
        string selectByIdQuery =
            """
            SELECT chat_id, sender_id, content, sent_at, is_read, is_deleted
            FROM messages
            WHERE id = @message_id
            """;

        using var selectCommand = new MySqlCommand(selectByIdQuery, conn);
        selectCommand.Parameters.AddWithValue("@message_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            message.ID = id;
            message.ChatID = reader.GetUInt32("chat_id");
            message.UserID = reader.GetUInt32("sender_id");
            message.MessageText = reader.GetString("content");
            message.TimeStamp = reader.GetDateTime("sent_at");
            message.IsRead = reader.GetBoolean("is_read");
            message.IsDeleted = reader.GetBoolean("is_deleted");
        }

        return message;
    }

    public static List<MessageModelID> SelectMessageModelsByUserId(uint userId, MySqlConnection conn)
    {
        List<MessageModelID> messages = [];
        string selectMessagesByUser =
            """
            SELECT id
            FROM messages
            WHERE sender_id = @user_id
            """;

        using var selectCommand = new MySqlCommand(selectMessagesByUser, conn);
        selectCommand.Parameters.AddWithValue("@user_id", userId);

        List<uint> messageIds = [];
        
        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            messageIds.Add(reader.GetUInt32("id"));
        }
        reader.Close();

        foreach (var messageId in messageIds)
        {
            messages.Add(SelectById(messageId, conn));
        }

        return messages;
    }
}