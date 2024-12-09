using System.Data;
using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

public static class TopicsService
{
    // public static List<TopicModel> SelectAll(MySqlConnection conn)
    // {
    //     List<TopicModel> topics = [];
    //     
    //     string selectAllQuery =
    //         """
    //         SELECT topics.id as topic_id, users.id as user_id, users.username, users.email, users.password, users.photo, 
    //                statuses.status, fields_of_study.field, specializations.specialization, universities.university, 
    //                term, degrees.degree, roles.role, users.created_at as user_created_at, is_banned, topics.title, 
    //                topics.content, topics.created_at as topic_created_at, topics.is_deleted, topics.votes,
    //                GROUP_CONCAT(topics_to_tags.tag) AS tags
    //         FROM topics
    //                  JOIN users ON topics.user_id = users.id
    //                  LEFT JOIN topics_to_tags ON topics.id = topics_to_tags.topic_id
    //                  LEFT JOIN statuses ON users.status_id = statuses.id
    //                  LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //                  LEFT JOIN specializations ON users.specialization_id = specializations.id
    //                  LEFT JOIN universities ON users.university_id = universities.id
    //                  LEFT JOIN degrees ON users.degree_id = degrees.id
    //                  JOIN roles ON users.role_id = roles.id
    //         WHERE topics.is_deleted = FALSE
    //         GROUP BY topics.id, users.id, users.username, users.email, users.password, users.photo, statuses.status,
    //                  fields_of_study.field, specializations.specialization, universities.university, term, 
    //                  degrees.degree, roles.role, users.created_at, is_banned, topics.title, topics.content, 
    //                  topics.created_at, topics.is_deleted, topics.votes
    //         """;
    //
    //     using var selectCommand = new MySqlCommand(selectAllQuery, conn);
    //     using var reader = selectCommand.ExecuteReader();
    //     
    //     while (reader.Read())
    //     {
    //         string? tagsString = reader.IsDBNull("tags") ? null : reader.GetString("tags");
    //         var tags = tagsString?.Split(',') ?? [];
    //         
    //         var topic = new TopicModel
    //         {
    //             ID = reader.GetUInt32("topic_id"),
    //             Author = new UserModel
    //             {
    //                 ID = reader.GetUInt32("user_id"),
    //                 Username = reader.GetString("username"),
    //                 Email = reader.GetString("email"),
    //                 Password = reader.GetString("password"),
    //                 Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
    //                 Status = reader.GetString("status"),
    //                 FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
    //                 Specialization = reader.IsDBNull("specialization") 
    //                     ? null 
    //                     : reader.GetString("specialization"),
    //                 Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
    //                 Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
    //                 University = reader.IsDBNull("university") ? null : reader.GetString("university"),
    //                 Role = reader.GetString("role"),
    //                 CreatedAt = reader.GetDateTime("user_created_at"),
    //                 IsBanned = reader.GetBoolean("is_banned")
    //             },
    //             Title = reader.GetString("title"),
    //             Content = reader.GetString("content"),
    //             CreatedAt = reader.GetDateTime("topic_created_at"),
    //             IsDeleted = reader.GetBoolean("is_deleted"),
    //             Rating = reader.GetInt32("votes"),
    //             Tags = tags.ToList()
    //         };
    //         
    //         topics.Add(topic);
    //     }
    //     
    //     reader.Close();
    //
    //     foreach (var topic in topics)
    //     {
    //         List<ReplyModel> replies = [];
    //         string selectRepliesQuery =
    //             """
    //             SELECT replies.id as reply_id, users.id as user_id, users.username, users.email, users.password, 
    //                    users.photo, statuses.status, fields_of_study.field, specializations.specialization, 
    //                    universities.university, term, degrees.degree, roles.role, users.created_at as user_created_at, 
    //                    users.is_banned, replies.topic_id, replies.parent_reply_id, replies.content, 
    //                    replies.created_at as reply_created_at, replies.votes, replies.is_deleted
    //             FROM replies
    //             JOIN users ON users.id = replies.user_id
    //             LEFT JOIN statuses ON users.status_id = statuses.id
    //             LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //             LEFT JOIN specializations ON users.specialization_id = specializations.id
    //             LEFT JOIN universities ON users.university_id = universities.id
    //             LEFT JOIN degrees ON users.degree_id = degrees.id
    //             LEFT JOIN roles ON users.role_id = roles.id
    //             WHERE topic_id = @topic_id AND replies.is_deleted = FALSE
    //             """;
    //
    //         using var selectRepliesCommand = new MySqlCommand(selectRepliesQuery, conn);
    //         selectRepliesCommand.Parameters.AddWithValue("@topic_id", topic.ID);
    //         
    //         using var repliesReader = selectRepliesCommand.ExecuteReader();
    //
    //         while (repliesReader.Read())
    //         {
    //             var reply = new ReplyModel
    //             {
    //                 ID = repliesReader.GetUInt32("reply_id"),
    //                 User = new UserModel
    //                 {
    //                     ID = repliesReader.GetUInt32("user_id"),
    //                     Username = repliesReader.GetString("username"),
    //                     Email = repliesReader.GetString("email"),
    //                     Password = repliesReader.GetString("password"),
    //                     Photo = repliesReader.IsDBNull("photo") ? null : repliesReader.GetString("photo"),
    //                     Status = repliesReader.GetString("status"),
    //                     FieldOfStudy = repliesReader.IsDBNull("field") 
    //                         ? null 
    //                         : repliesReader.GetString("field"),
    //                     Specialization = repliesReader.IsDBNull("specialization") 
    //                         ? null 
    //                         : repliesReader.GetString("specialization"),
    //                     Degree = repliesReader.IsDBNull("degree") 
    //                         ? null 
    //                         : repliesReader.GetString("degree"),
    //                     Term = repliesReader.IsDBNull("term") 
    //                         ? null 
    //                         : repliesReader.GetUInt32("term"),
    //                     University = repliesReader.IsDBNull("university") 
    //                         ? null 
    //                         : repliesReader.GetString("university"),
    //                     Role = repliesReader.GetString("role"),
    //                     CreatedAt = repliesReader.GetDateTime("user_created_at"),
    //                     IsBanned = repliesReader.GetBoolean("is_banned")
    //                 },
    //                 Topic = topic,
    //                 Content = repliesReader.GetString("content"),
    //                 CreatedAt = repliesReader.GetDateTime("reply_created_at"),
    //                 Rating = repliesReader.GetInt32("votes"),
    //                 IsDeleted = false
    //             };
    //
    //             uint? parentReplyId = repliesReader.IsDBNull("parent_reply_id")
    //                 ? null
    //                 : repliesReader.GetUInt32("parent_reply_id");
    //
    //             if (parentReplyId != null)
    //             {
    //                 foreach (var replyModel in replies.Where(replyModel => replyModel.ID == parentReplyId))
    //                 {
    //                     reply.ParentReply = replyModel;
    //                 }
    //             }
    //             
    //             replies.Add(reply);
    //         }
    //
    //         topic.Replies = replies;
    //     }
    //     
    //     return topics;
    // }
    
    // public static List<TopicModel> SelectPage(int pageNumber, int pageSize, MySqlConnection conn)
    // {
    //     List<TopicModel> topics = [];
    //
    //     int offset = (pageNumber - 1) * pageSize;
    //     
    //     string selectAllQuery =
    //         """
    //         SELECT topics.id as topic_id, users.id as user_id, users.username, users.email, users.password, users.photo, 
    //                statuses.status, fields_of_study.field, specializations.specialization, universities.university, 
    //                term, degrees.degree, roles.role, users.created_at as user_created_at, is_banned, topics.title, 
    //                topics.content, topics.created_at as topic_created_at, topics.is_deleted, topics.votes,
    //                GROUP_CONCAT(topics_to_tags.tag) AS tags
    //         FROM topics
    //                  JOIN users ON topics.user_id = users.id
    //                  LEFT JOIN topics_to_tags ON topics.id = topics_to_tags.topic_id
    //                  LEFT JOIN statuses ON users.status_id = statuses.id
    //                  LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //                  LEFT JOIN specializations ON users.specialization_id = specializations.id
    //                  LEFT JOIN universities ON users.university_id = universities.id
    //                  LEFT JOIN degrees ON users.degree_id = degrees.id
    //                  JOIN roles ON users.role_id = roles.id
    //         WHERE topics.is_deleted = FALSE
    //         GROUP BY topics.id, users.id, users.username, users.email, users.password, users.photo, statuses.status,
    //                  fields_of_study.field, specializations.specialization, universities.university, term, 
    //                  degrees.degree, roles.role, users.created_at, is_banned, topics.title, topics.content, 
    //                  topics.created_at, topics.is_deleted, topics.votes
    //         ORDER BY topic_created_at DESC
    //         LIMIT @page_size OFFSET @offset
    //         """;
    //
    //     using var selectCommand = new MySqlCommand(selectAllQuery, conn);
    //     selectCommand.Parameters.AddWithValue("@page_size", pageSize);
    //     selectCommand.Parameters.AddWithValue("@offset", offset);
    //     
    //     using var reader = selectCommand.ExecuteReader();
    //     
    //     while (reader.Read())
    //     {
    //         string? tagsString = reader.IsDBNull("tags") ? null : reader.GetString("tags");
    //         var tags = tagsString?.Split(',') ?? [];
    //         
    //         var topic = new TopicModel
    //         {
    //             ID = reader.GetUInt32("topic_id"),
    //             Author = new UserModel
    //             {
    //                 ID = reader.GetUInt32("user_id"),
    //                 Username = reader.GetString("username"),
    //                 Email = reader.GetString("email"),
    //                 Password = reader.GetString("password"),
    //                 Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
    //                 Status = reader.GetString("status"),
    //                 FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
    //                 Specialization = reader.IsDBNull("specialization") 
    //                     ? null 
    //                     : reader.GetString("specialization"),
    //                 Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
    //                 Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
    //                 University = reader.IsDBNull("university") ? null : reader.GetString("university"),
    //                 Role = reader.GetString("role"),
    //                 CreatedAt = reader.GetDateTime("user_created_at"),
    //                 IsBanned = reader.GetBoolean("is_banned")
    //             },
    //             Title = reader.GetString("title"),
    //             Content = reader.GetString("content"),
    //             CreatedAt = reader.GetDateTime("topic_created_at"),
    //             IsDeleted = reader.GetBoolean("is_deleted"),
    //             Rating = reader.GetInt32("votes"),
    //             Tags = tags.ToList()
    //         };
    //         
    //         topics.Add(topic);
    //     }
    //     
    //     reader.Close();
    //
    //     foreach (var topic in topics)
    //     {
    //         List<ReplyModel> replies = [];
    //         string selectRepliesQuery =
    //             """
    //             SELECT replies.id as reply_id, users.id as user_id, users.username, users.email, users.password, 
    //                    users.photo, statuses.status, fields_of_study.field, specializations.specialization, 
    //                    universities.university, term, degrees.degree, roles.role, users.created_at as user_created_at, 
    //                    users.is_banned, replies.topic_id, replies.parent_reply_id, replies.content, 
    //                    replies.created_at as reply_created_at, replies.votes, replies.is_deleted
    //             FROM replies
    //             JOIN users ON users.id = replies.user_id
    //             LEFT JOIN statuses ON users.status_id = statuses.id
    //             LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //             LEFT JOIN specializations ON users.specialization_id = specializations.id
    //             LEFT JOIN universities ON users.university_id = universities.id
    //             LEFT JOIN degrees ON users.degree_id = degrees.id
    //             LEFT JOIN roles ON users.role_id = roles.id
    //             WHERE topic_id = @topic_id AND replies.is_deleted = FALSE
    //             """;
    //
    //         using var selectRepliesCommand = new MySqlCommand(selectRepliesQuery, conn);
    //         selectRepliesCommand.Parameters.AddWithValue("@topic_id", topic.ID);
    //         
    //         using var repliesReader = selectRepliesCommand.ExecuteReader();
    //
    //         while (repliesReader.Read())
    //         {
    //             var reply = new ReplyModel
    //             {
    //                 ID = repliesReader.GetUInt32("reply_id"),
    //                 User = new UserModel
    //                 {
    //                     ID = repliesReader.GetUInt32("user_id"),
    //                     Username = repliesReader.GetString("username"),
    //                     Email = repliesReader.GetString("email"),
    //                     Password = repliesReader.GetString("password"),
    //                     Photo = repliesReader.IsDBNull("photo") ? null : repliesReader.GetString("photo"),
    //                     Status = repliesReader.GetString("status"),
    //                     FieldOfStudy = repliesReader.IsDBNull("field") 
    //                         ? null 
    //                         : repliesReader.GetString("field"),
    //                     Specialization = repliesReader.IsDBNull("specialization") 
    //                         ? null 
    //                         : repliesReader.GetString("specialization"),
    //                     Degree = repliesReader.IsDBNull("degree") 
    //                         ? null 
    //                         : repliesReader.GetString("degree"),
    //                     Term = repliesReader.IsDBNull("term") 
    //                         ? null 
    //                         : repliesReader.GetUInt32("term"),
    //                     University = repliesReader.IsDBNull("university") 
    //                         ? null 
    //                         : repliesReader.GetString("university"),
    //                     Role = repliesReader.GetString("role"),
    //                     CreatedAt = repliesReader.GetDateTime("user_created_at"),
    //                     IsBanned = repliesReader.GetBoolean("is_banned")
    //                 },
    //                 Topic = topic,
    //                 Content = repliesReader.GetString("content"),
    //                 CreatedAt = repliesReader.GetDateTime("reply_created_at"),
    //                 Rating = repliesReader.GetInt32("votes"),
    //                 IsDeleted = false
    //             };
    //
    //             uint? parentReplyId = repliesReader.IsDBNull("parent_reply_id")
    //                 ? null
    //                 : repliesReader.GetUInt32("parent_reply_id");
    //
    //             if (parentReplyId != null)
    //             {
    //                 foreach (var replyModel in replies.Where(replyModel => replyModel.ID == parentReplyId))
    //                 {
    //                     reply.ParentReply = replyModel;
    //                 }
    //             }
    //             
    //             replies.Add(reply);
    //         }
    //
    //         topic.Replies = replies;
    //     }
    //     
    //     return topics;
    // }
    
    public static bool Insert(TopicModel topic, MySqlConnection conn)
    {
        string insertQuery =
            """
            INSERT INTO topics (user_id, title, content, created_at, is_deleted, votes)
            VALUES (
                    (SELECT id FROM users WHERE username = @username LIMIT 1),
                    @title, @content, @created_at, @is_deleted, @votes
            )
            """;

        using var insertCommand = new MySqlCommand(insertQuery, conn);
        insertCommand.Parameters.AddWithValue("@username", topic.Author.Username); // TODO: verify UserModel
        insertCommand.Parameters.AddWithValue("@title", topic.Title);
        insertCommand.Parameters.AddWithValue("@content", topic.Content);
        insertCommand.Parameters.AddWithValue("@created_at", topic.CreatedAt);
        insertCommand.Parameters.AddWithValue("@is_deleted", topic.IsDeleted);
        insertCommand.Parameters.AddWithValue("@votes", topic.Rating);

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

    public static bool Delete(TopicModel topic, MySqlConnection conn)
    {
        string deleteQuery =
            $"""
             UPDATE topics
             SET is_deleted = TRUE
             WHERE id = @id
             """;

        using var updateCommand = new MySqlCommand(deleteQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", topic.ID);

        int affectedRows = updateCommand.ExecuteNonQuery();
        
        return affectedRows != 0;
    }

    // public static List<TopicModel> SelectByTag(string tag, MySqlConnection conn)
    // {
    //     List<TopicModel> topics = [];
    //     
    //     string selectAllQuery =
    //         """
    //         SELECT topics.id as topic_id, users.id as user_id, users.username, users.email, users.password, users.photo, 
    //                statuses.status, fields_of_study.field, specializations.specialization, universities.university, 
    //                term, degrees.degree, roles.role, users.created_at as user_created_at, is_banned, topics.title, 
    //                topics.content, topics.created_at as topic_created_at, topics.is_deleted, topics.votes,
    //                GROUP_CONCAT(topics_to_tags.tag) AS tags
    //         FROM topics
    //                  JOIN users ON topics.user_id = users.id
    //                  LEFT JOIN topics_to_tags ON topics.id = topics_to_tags.topic_id
    //                  LEFT JOIN statuses ON users.status_id = statuses.id
    //                  LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //                  LEFT JOIN specializations ON users.specialization_id = specializations.id
    //                  LEFT JOIN universities ON users.university_id = universities.id
    //                  LEFT JOIN degrees ON users.degree_id = degrees.id
    //                  JOIN roles ON users.role_id = roles.id
    //         WHERE topics.is_deleted = FALSE
    //         AND topics.id IN (SELECT topic_id FROM topics_to_tags WHERE tag = @tag) 
    //         GROUP BY topics.id, users.id, users.username, users.email, users.password, users.photo, statuses.status,
    //                  fields_of_study.field, specializations.specialization, universities.university, term, 
    //                  degrees.degree, roles.role, users.created_at, is_banned, topics.title, topics.content, 
    //                  topics.created_at, topics.is_deleted, topics.votes
    //         """;
    //
    //     using var selectCommand = new MySqlCommand(selectAllQuery, conn);
    //     selectCommand.Parameters.AddWithValue("@tag", tag);
    //     using var reader = selectCommand.ExecuteReader();
    //     
    //     while (reader.Read())
    //     {
    //         string? tagsString = reader.IsDBNull("tags") ? null : reader.GetString("tags");
    //         var tags = tagsString?.Split(',') ?? [];
    //         
    //         var topic = new TopicModel
    //         {
    //             ID = reader.GetUInt32("topic_id"),
    //             Author = new UserModel
    //             {
    //                 ID = reader.GetUInt32("user_id"),
    //                 Username = reader.GetString("username"),
    //                 Email = reader.GetString("email"),
    //                 Password = reader.GetString("password"),
    //                 Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
    //                 Status = reader.GetString("status"),
    //                 FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
    //                 Specialization = reader.IsDBNull("specialization") 
    //                     ? null 
    //                     : reader.GetString("specialization"),
    //                 Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
    //                 Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
    //                 University = reader.IsDBNull("university") ? null : reader.GetString("university"),
    //                 Role = reader.GetString("role"),
    //                 CreatedAt = reader.GetDateTime("user_created_at"),
    //                 IsBanned = reader.GetBoolean("is_banned")
    //             },
    //             Title = reader.GetString("title"),
    //             Content = reader.GetString("content"),
    //             CreatedAt = reader.GetDateTime("topic_created_at"),
    //             IsDeleted = reader.GetBoolean("is_deleted"),
    //             Rating = reader.GetInt32("votes"),
    //             Tags = tags.ToList()
    //         };
    //         
    //         topics.Add(topic);
    //     }
    //     
    //     reader.Close();
    //
    //     foreach (var topic in topics)
    //     {
    //         List<ReplyModel> replies = [];
    //         string selectRepliesQuery =
    //             """
    //             SELECT replies.id as reply_id, users.id as user_id, users.username, users.email, users.password, 
    //                    users.photo, statuses.status, fields_of_study.field, specializations.specialization, 
    //                    universities.university, term, degrees.degree, roles.role, users.created_at as user_created_at, 
    //                    users.is_banned, replies.topic_id, replies.parent_reply_id, replies.content, 
    //                    replies.created_at as reply_created_at, replies.votes, replies.is_deleted
    //             FROM replies
    //             JOIN users ON users.id = replies.user_id
    //             LEFT JOIN statuses ON users.status_id = statuses.id
    //             LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
    //             LEFT JOIN specializations ON users.specialization_id = specializations.id
    //             LEFT JOIN universities ON users.university_id = universities.id
    //             LEFT JOIN degrees ON users.degree_id = degrees.id
    //             LEFT JOIN roles ON users.role_id = roles.id
    //             WHERE topic_id = @topic_id AND replies.is_deleted = FALSE
    //             """;
    //
    //         using var selectRepliesCommand = new MySqlCommand(selectRepliesQuery, conn);
    //         selectRepliesCommand.Parameters.AddWithValue("@topic_id", topic.ID);
    //         
    //         using var repliesReader = selectRepliesCommand.ExecuteReader();
    //
    //         while (repliesReader.Read())
    //         {
    //             var reply = new ReplyModel
    //             {
    //                 ID = repliesReader.GetUInt32("reply_id"),
    //                 User = new UserModel
    //                 {
    //                     ID = repliesReader.GetUInt32("user_id"),
    //                     Username = repliesReader.GetString("username"),
    //                     Email = repliesReader.GetString("email"),
    //                     Password = repliesReader.GetString("password"),
    //                     Photo = repliesReader.IsDBNull("photo") ? null : repliesReader.GetString("photo"),
    //                     Status = repliesReader.GetString("status"),
    //                     FieldOfStudy = repliesReader.IsDBNull("field") 
    //                         ? null 
    //                         : repliesReader.GetString("field"),
    //                     Specialization = repliesReader.IsDBNull("specialization") 
    //                         ? null 
    //                         : repliesReader.GetString("specialization"),
    //                     Degree = repliesReader.IsDBNull("degree") 
    //                         ? null 
    //                         : repliesReader.GetString("degree"),
    //                     Term = repliesReader.IsDBNull("term") 
    //                         ? null 
    //                         : repliesReader.GetUInt32("term"),
    //                     University = repliesReader.IsDBNull("university") 
    //                         ? null 
    //                         : repliesReader.GetString("university"),
    //                     Role = repliesReader.GetString("role"),
    //                     CreatedAt = repliesReader.GetDateTime("user_created_at"),
    //                     IsBanned = repliesReader.GetBoolean("is_banned")
    //                 },
    //                 Topic = topic,
    //                 Content = repliesReader.GetString("content"),
    //                 CreatedAt = repliesReader.GetDateTime("reply_created_at"),
    //                 Rating = repliesReader.GetInt32("votes"),
    //                 IsDeleted = false
    //             };
    //
    //             uint? parentReplyId = repliesReader.IsDBNull("parent_reply_id")
    //                 ? null
    //                 : repliesReader.GetUInt32("parent_reply_id");
    //
    //             if (parentReplyId != null)
    //             {
    //                 foreach (var replyModel in replies.Where(replyModel => replyModel.ID == parentReplyId))
    //                 {
    //                     reply.ParentReply = replyModel;
    //                 }
    //             }
    //             
    //             replies.Add(reply);
    //         }
    //
    //         topic.Replies = replies;
    //     }
    //     
    //     return topics;
    // }

    public static bool Upvote(TopicModel topic, UserModel user, MySqlConnection conn)
    {
        string upvoteQuery =
            """
            UPDATE topics
            SET votes = votes + 1
            WHERE id = @topic_id
            """;

        using var updateCommand = new MySqlCommand(upvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@topic_id", topic.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false;
        
        return AttachVote(topic, user, 1, conn);
    }

    public static bool Downvote(TopicModel topic, UserModel user, MySqlConnection conn)
    {
        string downvoteQuery =
            """
            UPDATE topics
            SET votes = votes - 1
            WHERE id = @topic_id
            """;

        using var updateCommand = new MySqlCommand(downvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@topic_id", topic.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false;

        return AttachVote(topic, user, -1, conn);
    }

    private static bool AttachVote(TopicModel topic, UserModel user, int vote, MySqlConnection conn)
    {
        string attachVoteQuery = VoteExists(topic, user, conn)
            ? """
              UPDATE users_to_votes
              SET vote = @vote
              WHERE user_id = @user_id AND topic_id = @topic_id
              """
            : """
              INSERT INTO users_to_votes (user_id, topic_id, reply_id, vote)
              VALUES (@user_id, @topic_id, null, @vote)
              """;

        using var command = new MySqlCommand(attachVoteQuery, conn);
        command.Parameters.AddWithValue("@user_id", user.ID);
        command.Parameters.AddWithValue("@topic_id", topic.ID);
        command.Parameters.AddWithValue("@vote", vote);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    private static bool VoteExists(TopicModel topic, UserModel user, MySqlConnection conn)
    {
        string voteExistsQuery =
            """
            SELECT EXISTS 
                (SELECT 1 
                 FROM users_to_votes 
                 WHERE user_id = @user_id AND topic_id = @topic_id)
            """;

        using var selectCommand = new MySqlCommand(voteExistsQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", user.ID);
        selectCommand.Parameters.AddWithValue("@topic_id", topic.ID);

        return Convert.ToBoolean(selectCommand.ExecuteScalar());
    }
}