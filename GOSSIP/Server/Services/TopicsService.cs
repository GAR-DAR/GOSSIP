using System.Data;
using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

public static class TopicsService
{
    public static List<TopicModel> SelectAll(MySqlConnection conn)
    {
        List<TopicModel> topics = [];
        
        string selectAllQuery =
            """
            SELECT topics.id as topic_id, users.id as user_id, users.username, users.email, users.password, users.photo, 
                   statuses.status, fields_of_study.field, specializations.specialization, universities.university, 
                   term, degrees.degree, roles.role, users.created_at as user_created_at, is_banned, topics.title, 
                   topics.content, topics.created_at as topic_created_at, topics.is_deleted, topics.votes,
                   GROUP_CONCAT(topics_to_tags.tag) AS tags
            FROM topics
                     JOIN users ON topics.user_id = users.id
                     LEFT JOIN topics_to_tags ON topics.id = topics_to_tags.topic_id
                     LEFT JOIN statuses ON users.status_id = statuses.id
                     LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
                     LEFT JOIN specializations ON users.specialization_id = specializations.id
                     LEFT JOIN universities ON users.university_id = universities.id
                     LEFT JOIN degrees ON users.degree_id = degrees.id
                     JOIN roles ON users.role_id = roles.id
            WHERE topics.is_deleted = FALSE
            GROUP BY topics.id, users.id, users.username, users.email, users.password, users.photo, statuses.status,
                     fields_of_study.field, specializations.specialization, universities.university, term, 
                     degrees.degree, roles.role, users.created_at, is_banned, topics.title, topics.content, 
                     topics.created_at, topics.is_deleted, topics.votes
            """;
    
        using var selectCommand = new MySqlCommand(selectAllQuery, conn);
        using var reader = selectCommand.ExecuteReader();
        
        while (reader.Read())
        {
            string? tagsString = reader.IsDBNull("tags") ? null : reader.GetString("tags");
            var tags = tagsString?.Split(',') ?? [];
            
            var topic = new TopicModel
            {
                ID = reader.GetUInt32("topic_id"),
                Author = new UserModel
                {
                    ID = reader.GetUInt32("user_id"),
                    Username = reader.GetString("username"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password"),
                    Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
                    Status = reader.GetString("status"),
                    FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
                    Specialization = reader.IsDBNull("specialization") 
                        ? null 
                        : reader.GetString("specialization"),
                    Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
                    Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
                    University = reader.IsDBNull("university") ? null : reader.GetString("university"),
                    Role = reader.GetString("role"),
                    CreatedAt = reader.GetDateTime("user_created_at"),
                    IsBanned = reader.GetBoolean("is_banned")
                },
                Title = reader.GetString("title"),
                Content = reader.GetString("content"),
                CreatedAt = reader.GetDateTime("topic_created_at"),
                IsDeleted = reader.GetBoolean("is_deleted"),
                Rating = reader.GetInt32("votes"),
                Tags = tags.ToList()
            };
            
            topics.Add(topic);
        }
        
        reader.Close();
    
        foreach (var topic in topics)
        {
            List<ParentReplyModel> replies = [];
            string selectRepliesQuery =
                """
                SELECT replies.id as reply_id, 
                       
                       creator.id as creator_id, 
                       creator.username as creator_username, 
                       creator.email as creator_email, 
                       creator.photo as creator_photo, 
                       creator_statuses.status as creator_status, 
                       creator_fields_of_study.field as creator_field, 
                       creator_specializations.specialization as creator_specialization, 
                       creator_universities.university as creator_university, 
                       creator.term as creator_term, 
                       creator_degrees.degree as creator_degree, 
                       creator_roles.role as creator_role, 
                       creator.created_at as creator_created_at, 
                       creator.is_banned as creator_is_banned, 
                       
                       reply_to.id as reply_to_id,
                       reply_to.username as reply_to_username, 
                       reply_to.email as reply_to_email, 
                       reply_to.photo as reply_to_photo, 
                       reply_to_statuses.status as reply_to_status, 
                       reply_to_fields_of_study.field as reply_to_field, 
                       reply_to_specializations.specialization as reply_to_specialization, 
                       reply_to_universities.university as reply_to_university, 
                       reply_to.term as reply_to_term, 
                       reply_to_degrees.degree as reply_to_degree, 
                       reply_to_roles.role as reply_to_role, 
                       reply_to.created_at as reply_to_created_at, 
                       reply_to.is_banned as reply_to_is_banned, 
                       
                       replies.topic_id as reply_topic_id, 
                       replies.parent_reply_id as reply_parent_reply_id, 
                       replies.content as reply_content, 
                       replies.created_at as reply_created_at, 
                       replies.votes as reply_votes, 
                       replies.is_deleted as reply_is_deleted
                FROM replies
                JOIN users creator ON creator.id = replies.creator_id
                LEFT JOIN statuses creator_statuses 
                    ON creator.status_id = creator_statuses.id
                LEFT JOIN fields_of_study creator_fields_of_study 
                    ON creator.field_of_study_id = creator_fields_of_study.id
                LEFT JOIN specializations creator_specializations 
                    ON creator.specialization_id = creator_specializations.id
                LEFT JOIN universities creator_universities 
                    ON creator.university_id = creator_universities.id
                LEFT JOIN degrees creator_degrees 
                    ON creator.degree_id = creator_degrees.id
                LEFT JOIN roles creator_roles 
                    ON creator.role_id = creator_roles.id
                    
                LEFT JOIN users reply_to ON reply_to.id = replies.reply_to
                LEFT JOIN statuses reply_to_statuses 
                    ON reply_to.status_id = reply_to_statuses.id
                LEFT JOIN fields_of_study reply_to_fields_of_study 
                    ON reply_to.field_of_study_id = reply_to_fields_of_study.id
                LEFT JOIN specializations reply_to_specializations 
                    ON reply_to.specialization_id = reply_to_specializations.id
                LEFT JOIN universities reply_to_universities 
                    ON reply_to.university_id = reply_to_universities.id
                LEFT JOIN degrees reply_to_degrees 
                    ON reply_to.degree_id = reply_to_degrees.id
                LEFT JOIN roles reply_to_roles 
                    ON reply_to.role_id = reply_to_roles.id
                
                WHERE topic_id = @topic_id AND replies.is_deleted = FALSE
                ORDER BY reply_id
                """;
    
            using var selectRepliesCommand = new MySqlCommand(selectRepliesQuery, conn);
            selectRepliesCommand.Parameters.AddWithValue("@topic_id", topic.ID);
            
            using var repliesReader = selectRepliesCommand.ExecuteReader();
    
            while (repliesReader.Read())
            {
                ReplyModel reply = repliesReader.IsDBNull("reply_parent_reply_id")
                    ? new ParentReplyModel()
                    : new ChildReplyModel();

                reply.ID = repliesReader.GetUInt32("reply_id");
                reply.User = new UserModel
                {
                    ID = repliesReader.GetUInt32("creator_id"),
                    Username = repliesReader.GetString("creator_username"),
                    Email = repliesReader.GetString("creator_email"),
                    Photo = repliesReader.IsDBNull("creator_photo")
                        ? null
                        : repliesReader.GetString("creator_photo"),
                    Status = repliesReader.GetString("creator_status"),
                    FieldOfStudy = repliesReader.IsDBNull("creator_field")
                        ? null
                        : repliesReader.GetString("creator_field"),
                    Specialization = repliesReader.IsDBNull("creator_specialization")
                        ? null
                        : repliesReader.GetString("creator_specialization"),
                    Degree = repliesReader.IsDBNull("creator_degree")
                        ? null
                        : repliesReader.GetString("creator_degree"),
                    Term = repliesReader.IsDBNull("creator_term")
                        ? null
                        : repliesReader.GetUInt32("creator_term"),
                    University = repliesReader.IsDBNull("creator_university")
                        ? null
                        : repliesReader.GetString("creator_university"),
                    Role = repliesReader.GetString("creator_role"),
                    CreatedAt = repliesReader.GetDateTime("creator_created_at"),
                    IsBanned = repliesReader.GetBoolean("creator_is_banned")
                };
                reply.Topic = topic;
                reply.Content = repliesReader.GetString("reply_content");
                reply.CreatedAt = repliesReader.GetDateTime("reply_created_at");
                reply.Rating = repliesReader.GetInt32("reply_votes");
                reply.IsDeleted = repliesReader.GetBoolean("reply_is_deleted");
                
                if (reply is ParentReplyModel parentReply)
                    replies.Add(parentReply);

                if (reply is ChildReplyModel childReply)
                {
                    childReply.ReplyTo = new UserModel
                    {
                        ID = repliesReader.GetUInt32("reply_to_id"),
                        Username = repliesReader.GetString("reply_to_username"),
                        Email = repliesReader.GetString("reply_to_email"),
                        Photo = repliesReader.IsDBNull("reply_to_photo")
                            ? null
                            : repliesReader.GetString("reply_to_photo"),
                        Status = repliesReader.GetString("reply_to_status"),
                        FieldOfStudy = repliesReader.IsDBNull("reply_to_field")
                            ? null
                            : repliesReader.GetString("reply_to_field"),
                        Specialization = repliesReader.IsDBNull("reply_to_specialization")
                            ? null
                            : repliesReader.GetString("reply_to_specialization"),
                        Degree = repliesReader.IsDBNull("reply_to_degree")
                            ? null
                            : repliesReader.GetString("reply_to_degree"),
                        Term = repliesReader.IsDBNull("reply_to_term")
                            ? null
                            : repliesReader.GetUInt32("reply_to_term"),
                        University = repliesReader.IsDBNull("reply_to_university")
                            ? null
                            : repliesReader.GetString("reply_to_university"),
                        Role = repliesReader.GetString("reply_to_role"),
                        CreatedAt = repliesReader.GetDateTime("reply_to_created_at"),
                        IsBanned = repliesReader.GetBoolean("reply_to_is_banned")
                    };

                    foreach (var parentReplyModel in replies.Where(parentReplyModel => 
                                 parentReplyModel.ID == repliesReader.GetUInt32("reply_parent_reply_id")))
                    {
                        childReply.RootReply = parentReplyModel;
                        parentReplyModel.Replies.Add(childReply);
                    }
                }
            }
    
            topic.Replies = replies;
        }
        
        return topics;
    }
    
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

    public static bool Upvote(TopicModel topic, MySqlConnection conn)
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
        return rowsAffected != 0;
    }

    public static bool DownvoteTopic(TopicModel topic, MySqlConnection conn)
    {
        string upvoteQuery =
            """
            UPDATE topics
            SET votes = votes - 1
            WHERE id = @topic_id
            """;

        using var updateCommand = new MySqlCommand(upvoteQuery, conn);
        updateCommand.Parameters.AddWithValue("@topic_id", topic.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
}