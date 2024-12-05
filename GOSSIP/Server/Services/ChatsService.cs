using System.Data;
using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

// TODO: Delete a chat
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
                VALUES (@chat_id, (SELECT users.id FROM users WHERE users.username = @username))
                """;

            using var addUserCommand = new MySqlCommand(addUserQuery, conn);
            addUserCommand.Parameters.AddWithValue("@chat_id", insertCommand.LastInsertedId);
            addUserCommand.Parameters.AddWithValue("@username", user.Username);

            addUserCommand.ExecuteNonQuery();
        }

        return true;
    }

    public static bool AddUser(ChatModel chat, UserModel user, MySqlConnection conn)
    {
        string addUserQuery =
            """
            INSERT INTO chats_to_users (chat_id, user_id)
            VALUES (@chat_id, (SELECT users.id FROM users WHERE users.username = @username))
            """;

        using var insertCommand = new MySqlCommand(addUserQuery, conn);
        insertCommand.Parameters.AddWithValue("@chat_id", chat.ID);
        insertCommand.Parameters.AddWithValue("@username", user.Username);

        int rowsAffected = insertCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool DeleteUser(UserModel user, MySqlConnection conn)
    {
        string deleteUserQuery =
            """
            DELETE FROM chats_to_users WHERE user_id = (SELECT id FROM users WHERE username = @username)
            """;

        using var deleteCommand = new MySqlCommand(deleteUserQuery, conn);
        deleteCommand.Parameters.AddWithValue("@username", user.Username);

        int rowsAffected = deleteCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static List<ChatModel>? SelectByUser(UserModel user, MySqlConnection conn)
    {
        List<ChatModel> chats = [];
        
        string selectByUserQuery =
            """
            SELECT chats.id as chat_id, chats.name, chats.created_at as chat_created_at, 
            chats.is_deleted as chat_is_deleted
            FROM chats
            LEFT JOIN chats_to_users ON chats.id = chats_to_users.chat_id
            LEFT JOIN users ON chats_to_users.user_id = users.id
            WHERE chats.is_deleted = FALSE
            AND users.id = @user_id
            """;

        using var selectCommand = new MySqlCommand(selectByUserQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", user.ID);

        using var reader = selectCommand.ExecuteReader();
        while (reader.Read())
        {
            chats.Add(new ChatModel
            {
                ID = reader.GetUInt32("chat_id"),
                Name = reader.GetString("name"),
                CreatedAt = reader.GetDateTime("chat_created_at"),
                IsDeleted = reader.GetBoolean("chat_is_deleted")
            });
        }

        if (chats.Count == 0)
            return null;
        
        reader.Close();
        
        foreach (var chat in chats)
        {
            List<UserModel> users = [];

            string selectUsersQuery =
                """
                SELECT users.id, users.username, users.password, users.email, users.photo, statuses.status, 
                       fields_of_study.field, specializations.specialization, universities.university, term, 
                       degrees.degree, roles.role, created_at, is_banned
                FROM users
                LEFT JOIN statuses ON users.status_id = statuses.id
                LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
                LEFT JOIN specializations ON users.specialization_id = specializations.id
                LEFT JOIN universities ON users.university_id = universities.id
                LEFT JOIN degrees ON users.degree_id = degrees.id
                LEFT JOIN roles ON users.role_id = roles.id
                LEFT JOIN chats_to_users ON users.id = chats_to_users.user_id
                WHERE chats_to_users.chat_id = @chat_id
                """;

            using var selectUsersCommand = new MySqlCommand(selectUsersQuery, conn);
            selectUsersCommand.Parameters.AddWithValue("@chat_id", chat.ID);

            using var usersReader = selectUsersCommand.ExecuteReader();
            while (usersReader.Read())
            {
                users.Add(new UserModel
                {
                    ID = usersReader.GetUInt32("id"),
                    Username = usersReader.GetString("username"),
                    Email = usersReader.GetString("email"),
                    Photo = usersReader.IsDBNull("photo") ? null : usersReader.GetString("photo"),
                    Status = usersReader.GetString("status"),
                    FieldOfStudy = usersReader.IsDBNull("field") ? null : usersReader.GetString("field"),
                    Specialization = usersReader.IsDBNull("specialization") 
                        ? null 
                        : usersReader.GetString("specialization"),
                    Degree = usersReader.IsDBNull("degree") ? null : usersReader.GetString("degree"),
                    Term = usersReader.IsDBNull("term") ? null : usersReader.GetUInt32("term"),
                    University = usersReader.IsDBNull("university") 
                        ? null 
                        : usersReader.GetString("university"),
                    Role = usersReader.GetString("role"),
                    CreatedAt = usersReader.GetDateTime("created_at"),
                    IsBanned = usersReader.GetBoolean("is_banned")
                });
            }

            chat.Users = users;
            usersReader.Close();

            List<MessageModel> messages = [];
            string selectMessagesQuery =
                """
                SELECT messages.id as message_id, users.id as user_id, users.username, users.password, users.email, 
                       users.photo, statuses.status, fields_of_study.field, specializations.specialization, 
                       universities.university, term, degrees.degree, roles.role, users.created_at, users.is_banned,
                       messages.content, messages.sent_at, messages.is_read, messages.is_deleted
                FROM messages
                LEFT JOIN users ON messages.sender_id = users.id
                LEFT JOIN statuses ON users.status_id = statuses.id
                LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
                LEFT JOIN specializations ON users.specialization_id = specializations.id
                LEFT JOIN universities ON users.university_id = universities.id
                LEFT JOIN degrees ON users.degree_id = degrees.id
                LEFT JOIN roles ON users.role_id = roles.id
                WHERE chat_id = @chat_id
                AND messages.is_deleted = FALSE
                """;

            using var selectMessagesCommand = new MySqlCommand(selectMessagesQuery, conn);
            selectMessagesCommand.Parameters.AddWithValue("@chat_id", chat.ID);

            using var messagesReader = selectMessagesCommand.ExecuteReader();
            while (messagesReader.Read())
            {
                messages.Add(new MessageModel
                {
                    ID = messagesReader.GetUInt32("message_id"),
                    Chat = chat,
                    User = new UserModel
                    {
                        ID = messagesReader.GetUInt32("user_id"),
                        Username = messagesReader.GetString("username"),
                        Email = messagesReader.GetString("email"),
                        Photo = messagesReader.IsDBNull("photo") ? null : messagesReader.GetString("photo"),
                        Status = messagesReader.GetString("status"),
                        FieldOfStudy = messagesReader.IsDBNull("field") 
                            ? null 
                            : messagesReader.GetString("field"),
                        Specialization = messagesReader.IsDBNull("specialization") 
                            ? null 
                            : messagesReader.GetString("specialization"),
                        Degree = messagesReader.IsDBNull("degree") ? null : messagesReader.GetString("degree"),
                        Term = messagesReader.IsDBNull("term") ? null : messagesReader.GetUInt32("term"),
                        University = messagesReader.IsDBNull("university") 
                            ? null 
                            : messagesReader.GetString("university"),
                        Role = messagesReader.GetString("role"),
                        CreatedAt = messagesReader.GetDateTime("created_at"),
                        IsBanned = messagesReader.GetBoolean("is_banned")
                    },
                    MessageText = messagesReader.GetString("content"),
                    IsRead = messagesReader.GetBoolean("is_read"),
                    IsDeleted = messagesReader.GetBoolean("is_deleted")
                });
            }

            chat.Messages = messages;
        }

        return chats;
    }
}