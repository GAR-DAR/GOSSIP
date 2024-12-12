using System.Data;
using MySql.Data.MySqlClient;
using Server;

namespace Server.Services;

// TODO: search by title
public static class TopicsService
{
    public static List<uint> SelectAllIds(MySqlConnection conn)
    {
        List<uint> topicIds = [];

        string selectAllQuery =
            """
            SELECT id
            FROM topics
            WHERE is_deleted = FALSE
            """;

        using var selectCommand = new MySqlCommand(selectAllQuery, conn);
        using var reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {

            topicIds.Add(reader.GetUInt32("id"));
        }

        return topicIds;
    }

    public static List<TopicModelID> SelectAll(MySqlConnection conn)
    {
        List<TopicModelID> topics = [];
        List<uint> topicIds = SelectAllIds(conn);

        foreach (var topicId in topicIds)
        {
            topics.Add(SelectById(topicId, conn));
        }

        return topics;
    }

    public static bool Insert(TopicModelID topic, MySqlConnection conn)
    {
        string insertQuery =
            """
            INSERT INTO topics (user_id, title, content, created_at, is_deleted, votes)
            VALUES (@user_id, @title, @content, NOW(), FALSE, 0)
            """;

        using var insertCommand = new MySqlCommand(insertQuery, conn);
        insertCommand.Parameters.AddWithValue("@user_id", topic.AuthorID);
        insertCommand.Parameters.AddWithValue("@title", topic.Title);
        insertCommand.Parameters.AddWithValue("@content", topic.Content);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false;

        if (topic.Tags.Count == 0)
            return true;

        int topicId = (int)insertCommand.LastInsertedId;
        string insertTagQuery =
            $"""
             INSERT INTO topics_to_tags (topic_id, tag)
             VALUES (@topic_id, @tag)
             """;

        using var insertTagCommand = new MySqlCommand(insertTagQuery, conn);
        insertTagCommand.Parameters.AddWithValue("@topic_id", topicId);
        insertTagCommand.Parameters.Add("@tag", MySqlDbType.VarChar, 255);

        foreach (var tag in topic.Tags)
        {
            insertTagCommand.Parameters["@tag"].Value = tag;
            insertTagCommand.ExecuteNonQuery();
        }

        return true;
    }

    public static bool Delete(uint id, MySqlConnection conn)
    {
        string deleteQuery =
            $"""
             UPDATE topics
             SET is_deleted = TRUE
             WHERE id = @id
             """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", id);

        int affectedRows = updateCommand.ExecuteNonQuery();
        return affectedRows != 0;
    }

    public static TopicModelID SelectById(uint id, MySqlConnection conn)
    {
        var topic = new TopicModelID();
        string selectByIdQuery =
            """
            SELECT user_id, title, content, created_at, is_deleted, votes
            FROM topics
            WHERE id = @topic_id
            """;

        using var selectCommand = new MySqlCommand(selectByIdQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            topic.ID = id;
            topic.AuthorID = reader.GetUInt32("user_id");
            topic.Title = reader.GetString("title");
            topic.Content = reader.GetString("content");
            topic.CreatedAt = reader.GetDateTime("created_at");
            topic.IsDeleted = reader.GetBoolean("is_deleted");
            topic.Rating = reader.GetInt32("votes");
        }

        reader.Close();

        topic.Replies = SelectParentReplyIdsByTopic(id, conn);
        topic.Tags = SelectTagsByTopic(id, conn);

        return topic;
    }

    public static List<uint> SelectParentReplyIdsByTopic(uint topicId, MySqlConnection conn)
    {
        List<uint> replyIds = [];
        string selectReplyIdsQuery =
            """
            SELECT id
            FROM replies
            WHERE topic_id = @topic_id
            AND is_deleted = FALSE
            AND parent_reply_id IS NULL
            """;

        using var selectCommand = new MySqlCommand(selectReplyIdsQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", topicId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            replyIds.Add(reader.GetUInt32("id"));
        }

        return replyIds;
    }

    public static List<ParentReplyModelID> SelectParentRepliesByTopic(uint topicId, MySqlConnection conn)
    {
        List<ParentReplyModelID> parentReplies = [];
        List<uint> replyIds = [];
        string selectReplyIdsQuery =
            """
            SELECT id
            FROM replies
            WHERE topic_id = @topic_id
            AND is_deleted = FALSE
            AND parent_reply_id IS NULL
            """;

        using var selectCommand = new MySqlCommand(selectReplyIdsQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", topicId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            replyIds.Add(reader.GetUInt32("id"));
        }

        reader.Close();

        foreach (var replyId in replyIds)
        {
            parentReplies.Add((ParentReplyModelID)RepliesService.SelectById(replyId, conn));
        }

        return parentReplies;
    }

    public static List<ChildReplyModelID> SelectChildRepliesByTopic(uint topicId, MySqlConnection conn)
    {
        List<ChildReplyModelID> childReplies = [];
        List<uint> replyIds = [];
        string selectReplyIdsQuery =
            """
            SELECT id
            FROM replies
            WHERE topic_id = @topic_id
            AND is_deleted = FALSE
            AND parent_reply_id IS NOT NULL
            """;

        using var selectCommand = new MySqlCommand(selectReplyIdsQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", topicId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            replyIds.Add(reader.GetUInt32("id"));
        }

        reader.Close();

        foreach (var replyId in replyIds)
        {
            childReplies.Add((ChildReplyModelID)RepliesService.SelectById(replyId, conn));
        }

        return childReplies;
    }

    public static List<string> SelectTagsByTopic(uint topicId, MySqlConnection conn)
    {
        List<string> tags = [];
        string selectTagsQuery =
            """
            SELECT tag
            FROM topics_to_tags
            WHERE topic_id = @topic_id
            """;

        using var selectCommand = new MySqlCommand(selectTagsQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", topicId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            tags.Add(reader.GetString("tag"));
        }

        return tags;
    }

    public static List<TopicModelID> SelectTopicsByUser(uint userId, MySqlConnection conn)
    {
        List<TopicModelID> topics = [];
        List<uint> topicIds = [];
        string selectTopicsByUserQuery =
            """
            SELECT id FROM topics WHERE user_id = @user_id
            """;

        using var selectCommand = new MySqlCommand(selectTopicsByUserQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", userId);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            topicIds.Add(reader.GetUInt32("id"));
        }
        reader.Close();

        foreach (var topicId in topicIds)
        {
            topics.Add(SelectById(topicId, conn));
        }

        return topics;
    }

    public static List<TopicModelID> SelectTopicsByTag(string tag, MySqlConnection conn)
    {
        List<TopicModelID> topics = [];
        List<uint> topicIds = [];
        string selectTopicsByTagQuery =
            """
            SELECT topic_id FROM topics_to_tags WHERE tag = @tag ORDER BY votes DESC
            """;

        using var selectCommand = new MySqlCommand(selectTopicsByTagQuery, conn);
        selectCommand.Parameters.AddWithValue("@tag", tag);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            topicIds.Add(reader.GetUInt32("topic_id"));
        }
        reader.Close();

        foreach (var topicId in topicIds)
        {
            topics.Add(SelectById(topicId, conn));
        }

        return topics;
    }

    public static List<TopicModelID> SelectTopicsByTitle(string searchQuery, MySqlConnection conn)
    {
        List<TopicModelID> topics = [];
        List<uint> topicIds = [];
        string[] searchWords = searchQuery.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
        string searchTopicsByTitleQuery =
            """
            SELECT id 
            FROM topics
            WHERE title LIKE @word
            ORDER BY votes DESC
            """;

        using var selectCommand = new MySqlCommand(searchTopicsByTitleQuery, conn);
        selectCommand.Parameters.Add("@word", MySqlDbType.VarChar, 255);

        foreach (var word in searchWords)
        {
            selectCommand.Parameters["@word"].Value = "%" + word + "%";
            using var reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                var topicId = reader.GetUInt32("id");
                if (!topicIds.Contains(topicId))
                    topicIds.Add(topicId);
            }
        }

        foreach (var topicId in topicIds)
        {
            topics.Add(SelectById(topicId, conn));
        }

        return topics;
    }

    public static bool Upvote(uint id, MySqlConnection conn)
    {
        string upvoteQuery =
            """
            UPDATE topics
            SET votes = votes + 1
            WHERE id = @topic_id
            """;

        using var updateCommand = new MySqlCommand(upvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@topic_id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool Downvote(uint id, MySqlConnection conn)
    {
        string downvoteQuery =
            """
            UPDATE topics
            SET votes = votes - 1
            WHERE id = @topic_id
            """;

        using var updateCommand = new MySqlCommand(downvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@topic_id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static List<ReplyModelID> SelectAllRepliesByTopic(uint id, MySqlConnection conn)
    {
        List<ReplyModelID> replies = [];
        List<uint> replyIds = [];
        string selectAllRepliesQuery =
            """
            SELECT id FROM replies WHERE topic_id = @topic_id
            """;

        using var selectCommand = new MySqlCommand(selectAllRepliesQuery, conn);
        selectCommand.Parameters.AddWithValue("@topic_id", id);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            replyIds.Add(reader.GetUInt32("id"));
        }
        reader.Close();

        foreach (var replyId in replyIds)
        {
            replies.Add(RepliesService.SelectById(replyId, conn));
        }

        return replies;
    }
}