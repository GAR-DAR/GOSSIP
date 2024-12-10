using MySql.Data.MySqlClient;
using System.Data;
using Server.Models;

namespace Server.Services;

public static class RepliesService
{
    public static bool Add(uint userId, uint topicId, uint? rootReplyId, uint? replyToId, string content, 
        MySqlConnection conn)
    {
        string addQuery =
            """
            INSERT INTO replies (creator_id, topic_id, parent_reply_id, reply_to, content, created_at, votes, is_deleted)
            VALUES (@user_id, @topic_id, @parent_reply_id, @reply_to, @content, NOW(), 0, FALSE)
            """;

        using var insertCommand = new MySqlCommand(addQuery, conn);
        insertCommand.Parameters.AddWithValue("@user_id", userId);
        insertCommand.Parameters.AddWithValue("@topic_id", topicId);
        insertCommand.Parameters.AddWithValue("@parent_reply_id", rootReplyId);
        insertCommand.Parameters.AddWithValue("@reply_to", replyToId);
        insertCommand.Parameters.AddWithValue("@content", content);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    // public static bool Upvote(ReplyModelID reply, UserModelID user, MySqlConnection conn)
    // {
    //     string upvoteQuery =
    //         """
    //         UPDATE replies
    //         SET votes = votes + 1
    //         WHERE id = @reply_id
    //         """;
    //
    //     using var updateCommand = new MySqlCommand(upvoteQuery, conn);
    //     updateCommand.Parameters.AddWithValue("@reply_id", reply.ID);
    //
    //     int rowsAffected = updateCommand.ExecuteNonQuery();
    //     if (rowsAffected == 0)
    //         return false;
    //
    //     return AttachVote(reply, user, 1, conn);
    // }
    //
    // public static bool Downvote(ReplyModelID reply, UserModelID user, MySqlConnection conn)
    // {
    //     string downvoteQuery =
    //         """
    //         UPDATE replies
    //         SET votes = votes - 1
    //         WHERE id = @reply_id
    //         """;
    //
    //     using var updateCommand = new MySqlCommand(downvoteQuery, conn);
    //     updateCommand.Parameters.AddWithValue("@reply_id", reply.ID);
    //
    //     int rowsAffected = updateCommand.ExecuteNonQuery();
    //     if (rowsAffected == 0)
    //         return false;
    //
    //     return AttachVote(reply, user, -1, conn);
    // }
    
    public static bool Delete(uint id, MySqlConnection conn)
    {
        string deleteQuery =
            $"""
             UPDATE replies
             SET is_deleted = TRUE
             WHERE id = @id
             """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", id);

        int affectedRows = updateCommand.ExecuteNonQuery();
        return affectedRows != 0;
    }

    public static ReplyModelID SelectById(uint id, MySqlConnection conn)
    {
        string selectByIdQuery =
            """
            SELECT creator_id, topic_id, parent_reply_id, reply_to, content, created_at, votes, is_deleted
            FROM replies
            WHERE id = @reply_id
            """;

        using var selectCommand = new MySqlCommand(selectByIdQuery, conn);
        selectCommand.Parameters.AddWithValue("@reply_id", id);

        using var reader = selectCommand.ExecuteReader();
        reader.Read();

        ReplyModelID reply = reader.IsDBNull("parent_reply_id")
            ? new ParentReplyModelID()
            : new ChildReplyModelID();

        reply.ID = id;
        reply.UserID = reader.GetUInt32("creator_id");
        reply.TopicID = reader.GetUInt32("topic_id");
        reply.Content = reader.GetString("content");
        reply.CreatedAt = reader.GetDateTime("created_at");
        reply.Rating = reader.GetInt32("votes");
        reply.IsDeleted = reader.GetBoolean("is_deleted");

        if (reply is ChildReplyModelID childReply)
        {
            childReply.RootReplyID = reader.GetUInt32("parent_reply_id");
            childReply.ReplyToUserID = reader.GetUInt32("reply_to");
        }

        if (reply is ParentReplyModelID parentReply)
        {
            reader.Close();

            parentReply.Replies = SelectChildReplyIdsByParent(id, conn);
        }

        return reply;
    }

    public static List<uint> SelectChildReplyIdsByParent(uint id, MySqlConnection conn)
    {
        List<uint> childReplyIds = [];
        string selectChildRepliesQuery =
            """
            SELECT id
            FROM replies
            WHERE parent_reply_id = @parent_reply_id
            """;

        using var selectCommand = new MySqlCommand(selectChildRepliesQuery, conn);
        selectCommand.Parameters.AddWithValue("@parent_reply_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            childReplyIds.Add(reader.GetUInt32("id"));
        }

        return childReplyIds;
    }
    
    public static List<ChildReplyModelID> SelectChildRepliesByParent(uint id, MySqlConnection conn)
    {
        List<ChildReplyModelID> childReplies = [];
        List<uint> childReplyIds = [];
        string selectChildRepliesQuery =
            """
            SELECT id
            FROM replies
            WHERE parent_reply_id = @parent_reply_id
            """;

        using var selectCommand = new MySqlCommand(selectChildRepliesQuery, conn);
        selectCommand.Parameters.AddWithValue("@parent_reply_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            childReplyIds.Add(reader.GetUInt32("id"));
        }
        
        reader.Close();

        foreach (var childReplyId in childReplyIds)
        {
            childReplies.Add((ChildReplyModelID)SelectById(childReplyId, conn));
        }

        return childReplies;
    }
    
    // private static bool AttachVote(ReplyModelID reply, UserModelID user, int vote, MySqlConnection conn)
    // {
    //     string attachVoteQuery = VoteExists(reply, user, conn)
    //         ? """
    //           UPDATE users_to_votes
    //           SET vote = @vote
    //           WHERE user_id = @user_id AND reply_id = @reply_id
    //           """
    //         : """
    //           INSERT INTO users_to_votes (user_id, topic_id, reply_id, vote)
    //           VALUES (@user_id, null, @reply_id, @vote)
    //           """;
    //
    //     using var command = new MySqlCommand(attachVoteQuery, conn);
    //     command.Parameters.AddWithValue("@user_id", user.ID);
    //     command.Parameters.AddWithValue("@reply_id", reply.ID);
    //     command.Parameters.AddWithValue("@vote", vote);
    //
    //     int rowsAffected = command.ExecuteNonQuery();
    //     return rowsAffected != 0;
    // }
    //
    // private static bool VoteExists(ReplyModelID reply, UserModelID user, MySqlConnection conn)
    // {
    //     string voteExistsQuery =
    //         """
    //         SELECT EXISTS 
    //             (SELECT 1 
    //              FROM users_to_votes 
    //              WHERE user_id = @user_id AND reply_id = @reply_id)
    //         """;
    //
    //     using var selectCommand = new MySqlCommand(voteExistsQuery, conn);
    //     selectCommand.Parameters.AddWithValue("@user_id", user.ID);
    //     selectCommand.Parameters.AddWithValue("@reply_id", reply.ID);
    //
    //     return Convert.ToBoolean(selectCommand.ExecuteScalar());
    // }
}