using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

// TODO: delete reply

public static class RepliesService
{
    public static bool Add(ReplyModel reply, MySqlConnection conn)
    {
        string addQuery =
            """
            INSERT INTO replies (user_id, topic_id, parent_reply_id, content, created_at, votes, is_deleted)
            VALUES (@user_id, @topic_id, @parent_reply_id, @content, @created_at, @votes, @is_deleted)
            """;

        using var insertCommand = new MySqlCommand(addQuery, conn);
        insertCommand.Parameters.AddWithValue("@user_id", reply.User.ID);
        insertCommand.Parameters.AddWithValue("@topic_id", reply.Topic.ID);
        //insertCommand.Parameters.AddWithValue("@parent_reply_id", reply.ParentReply?.ID);
        insertCommand.Parameters.AddWithValue("@content", reply.Content);
        insertCommand.Parameters.AddWithValue("@created_at", reply.CreatedAt);
        insertCommand.Parameters.AddWithValue("@votes", reply.Rating);
        insertCommand.Parameters.AddWithValue("@is_deleted", reply.IsDeleted);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool Upvote(ReplyModel reply, MySqlConnection conn)
    {
        string upvoteQuery =
            """
            UPDATE replies
            SET votes = votes + 1
            WHERE id = @reply_id
            """;

        using var updateCommand = new MySqlCommand(upvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@reply_id", reply.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
    
    public static bool Downvote(ReplyModel reply, MySqlConnection conn)
    {
        string downvoteQuery =
            """
            UPDATE replies
            SET votes = votes - 1
            WHERE id = @reply_id
            """;

        using var updateCommand = new MySqlCommand(downvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@reply_id", reply.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
    
    public static bool Delete(ReplyModel reply, MySqlConnection conn)
    {
        string deleteQuery =
            $"""
             UPDATE replies
             SET is_deleted = TRUE
             WHERE id = @id
             """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", reply.ID);

        int affectedRows = updateCommand.ExecuteNonQuery();
        
        return affectedRows != 0;
    }
}