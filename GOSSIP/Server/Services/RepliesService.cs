using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

public static class RepliesService
{
    public static bool Add(ReplyModel reply, MySqlConnection conn)
    {
        string addQuery =
            """
            INSERT INTO replies (creator_id, topic_id, parent_reply_id, reply_to, content, created_at, votes, is_deleted)
            VALUES (@user_id, @topic_id, @parent_reply_id, @reply_to, @content, @created_at, @votes, @is_deleted)
            """;

        using var insertCommand = new MySqlCommand(addQuery, conn);
        insertCommand.Parameters.AddWithValue("@user_id", reply.User.ID);
        insertCommand.Parameters.AddWithValue("@topic_id", reply.Topic.ID);
        insertCommand.Parameters.AddWithValue("@parent_reply_id",
            (reply is ChildReplyModel childReplyRoot) ? childReplyRoot.RootReply.ID : null);
        insertCommand.Parameters.AddWithValue("@reply_to",
            (reply is ChildReplyModel childReplyReplyTo) ? childReplyReplyTo.ReplyTo.ID : null);
        insertCommand.Parameters.AddWithValue("@content", reply.Content);
        insertCommand.Parameters.AddWithValue("@created_at", reply.CreatedAt);
        insertCommand.Parameters.AddWithValue("@votes", reply.Rating);
        insertCommand.Parameters.AddWithValue("@is_deleted", reply.IsDeleted);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool Upvote(ReplyModel reply, UserModel user, MySqlConnection conn)
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
        if (rowsAffected == 0)
            return false;

        return AttachVote(reply, user, 1, conn);
    }

    public static bool Downvote(ReplyModel reply, UserModel user, MySqlConnection conn)
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
        if (rowsAffected == 0)
            return false;

        return AttachVote(reply, user, -1, conn);
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

    private static bool AttachVote(ReplyModel reply, UserModel user, int vote, MySqlConnection conn)
    {
        string attachVoteQuery = VoteExists(reply, user, conn)
            ? """
              UPDATE users_to_votes
              SET vote = @vote
              WHERE user_id = @user_id AND reply_id = @reply_id
              """
            : """
              INSERT INTO users_to_votes (user_id, topic_id, reply_id, vote)
              VALUES (@user_id, null, @reply_id, @vote)
              """;

        using var command = new MySqlCommand(attachVoteQuery, conn);
        command.Parameters.AddWithValue("@user_id", user.ID);
        command.Parameters.AddWithValue("@reply_id", reply.ID);
        command.Parameters.AddWithValue("@vote", vote);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    private static bool VoteExists(ReplyModel reply, UserModel user, MySqlConnection conn)
    {
        string voteExistsQuery =
            """
            SELECT EXISTS 
                (SELECT 1 
                 FROM users_to_votes 
                 WHERE user_id = @user_id AND reply_id = @reply_id)
            """;

        using var selectCommand = new MySqlCommand(voteExistsQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", user.ID);
        selectCommand.Parameters.AddWithValue("@reply_id", reply.ID);

        return Convert.ToBoolean(selectCommand.ExecuteScalar());
    }
}