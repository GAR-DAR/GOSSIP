using System.Data;
using MySql.Data.MySqlClient;
using Server;

namespace Server.Services;

public static class UsersService
{
    public static UserModelID? SignUp(UserModelID user, MySqlConnection conn)
    {
        if (Exists("email", user.Email, conn)
            || Exists("username", user.Username, conn))
            return null; // TODO: an exception, for sure

        string signUpQuery =
            $"""
             INSERT INTO users (username, email, password, photo, status_id, field_of_study_id, specialization_id, 
                                university_id, term, degree_id, role_id, created_at, is_banned)
             VALUES (
                     @username, @email, @password, @photo,
                     (SELECT id FROM statuses WHERE status = @status LIMIT 1),
                     (SELECT id FROM fields_of_study WHERE field = @field LIMIT 1),
                     (SELECT id FROM specializations WHERE specialization = @specialization LIMIT 1),
                     (SELECT id FROM universities WHERE university = @university LIMIT 1),
                     @term,
                     (SELECT id FROM degrees WHERE degree = @degree LIMIT 1),
                     (SELECT id FROM roles WHERE role = @role LIMIT 1),
                     NOW(), false
                     )
             """;

        using var insertCommand = new MySqlCommand(signUpQuery, conn);
        insertCommand.Parameters.AddWithValue("@username", user.Username);
        insertCommand.Parameters.AddWithValue("@email", user.Email);
        insertCommand.Parameters.AddWithValue("@password", user.Password);
        insertCommand.Parameters.AddWithValue("@photo", user.Photo);
        insertCommand.Parameters.AddWithValue("@status", user.Status);
        insertCommand.Parameters.AddWithValue("@field", user.FieldOfStudy);
        insertCommand.Parameters.AddWithValue("@specialization", user.Specialization);
        insertCommand.Parameters.AddWithValue("@university", user.University);
        insertCommand.Parameters.AddWithValue("@term", user.Term);
        insertCommand.Parameters.AddWithValue("@degree", user.Degree);
        insertCommand.Parameters.AddWithValue("@role", user.Role);

        int affectedRows = insertCommand.ExecuteNonQuery();
        if (affectedRows == 0)
            return null;

        user.ID = (uint)insertCommand.LastInsertedId;
        return user;
    }

    public static UserModelID? SignIn(string? email, string? username, string password, MySqlConnection conn)
    {
        string typeOfLogin = (email == null) ? "username" : "email";
        string login = (email == null) ? username : email;
        // typeOfLogin: email || username
        if (!Exists(typeOfLogin, login, conn))
            return null; // TODO: may be replaced with an exception

        string signInQuery =
            $"""
             SELECT users.id, users.username, users.password, users.email, users.photo, statuses.status, 
                    fields_of_study.field, specializations.specialization, universities.university, term, degrees.degree, 
                    roles.role, created_at, is_banned
             FROM users
             LEFT JOIN statuses ON users.status_id = statuses.id
             LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
             LEFT JOIN specializations ON users.specialization_id = specializations.id
             LEFT JOIN universities ON users.university_id = universities.id
             LEFT JOIN degrees ON users.degree_id = degrees.id
             LEFT JOIN roles ON users.role_id = roles.id
             WHERE {typeOfLogin} = @value
             """;

        using var selectCommand = new MySqlCommand(signInQuery, conn);
        selectCommand.Parameters.AddWithValue("@value", login);

        using var reader = selectCommand.ExecuteReader();
        reader.Read();

        string storedPassword = reader.GetString("password");
        if (password != storedPassword)
            return null; // TODO: exception is begging to be thrown here

        bool isBanned = reader.GetBoolean("is_banned");
        if (isBanned)
            return null;

        var user = new UserModelID
        {
            ID = reader.GetUInt32("id"),
            Username = reader.GetString("username"),
            Email = reader.GetString("email"),
            Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
            Status = reader.GetString("status"),
            FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
            Specialization = reader.IsDBNull("specialization") ? null : reader.GetString("specialization"),
            Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
            Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
            University = reader.IsDBNull("university") ? null : reader.GetString("university"),
            Role = reader.GetString("role"),
            CreatedAt = reader.GetDateTime("created_at"),
            IsBanned = false
        };

        reader.Close();

        user.ChatsID = ChatsService.SelectChatIdsByUser(user.ID, conn);

        return user;
    }

    public static bool Exists(string findBy, string value, MySqlConnection conn)
    {
        string existsQuery =
            $"""
             SELECT EXISTS (SELECT 1 FROM users WHERE {findBy} = @value)
             """;

        using var selectCommand = new MySqlCommand(existsQuery, conn);
        selectCommand.Parameters.AddWithValue("@value", value);
        return Convert.ToBoolean(selectCommand.ExecuteScalar());
    }

    public static UserModelID SelectById(uint userId, MySqlConnection conn)
    {
        string selectQuery =
            """
            SELECT users.id, users.username, users.email, users.password, users.photo, statuses.status, 
            fields_of_study.field, specializations.specialization, universities.university, users.term, 
            degrees.degree, roles.role, users.created_at, users.is_banned
            FROM users 
            LEFT JOIN statuses ON users.status_id = statuses.id
            LEFT JOIN fields_of_study ON users.field_of_study_id = fields_of_study.id
            LEFT JOIN specializations ON users.specialization_id = specializations.id
            LEFT JOIN universities ON users.university_id = universities.id
            LEFT JOIN degrees ON users.degree_id = degrees.id
            LEFT JOIN roles ON users.role_id = roles.id
            WHERE users.id = @user_id
            """;

        using var selectCommand = new MySqlCommand(selectQuery, conn);
        selectCommand.Parameters.AddWithValue("@user_id", userId);

        using var reader = selectCommand.ExecuteReader();
        reader.Read();

        var user = new UserModelID
        {
            ID = userId,
            Username = reader.GetString("username"),
            Email = reader.GetString("email"),
            Photo = reader.IsDBNull("photo") ? null : reader.GetString("photo"),
            Status = reader.GetString("status"),
            FieldOfStudy = reader.IsDBNull("field") ? null : reader.GetString("field"),
            Specialization = reader.IsDBNull("specialization") ? null : reader.GetString("specialization"),
            Degree = reader.IsDBNull("degree") ? null : reader.GetString("degree"),
            Term = reader.IsDBNull("term") ? null : reader.GetUInt32("term"),
            University = reader.IsDBNull("university") ? null : reader.GetString("university"),
            Role = reader.GetString("role"),
            CreatedAt = reader.GetDateTime("created_at"),
            IsBanned = reader.GetBoolean("is_banned")
        };

        reader.Close();

        user.ChatsID = ChatsService.SelectChatIdsByUser(user.ID, conn);
        // user.TopicVotes = GetTopicVotes(user, conn);
        // user.ReplyVotes = GetReplyVotes(user, conn);

        return user;
    }

    public static List<uint> SelectAllIds(MySqlConnection conn)
    {
        List<uint> userIds = [];
        string selectQuery =
            """
            SELECT id FROM users
            """;

        using var selectCommand = new MySqlCommand(selectQuery, conn);
        using var reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            userIds.Add(reader.GetUInt32("id"));
        }

        return userIds;
    }

    public static List<UserModelID> SelectAll(MySqlConnection conn)
    {
        List<UserModelID> users = [];
        List<uint> userIds = [];
        string selectQuery =
            """
            SELECT id FROM users
            """;

        using var selectCommand = new MySqlCommand(selectQuery, conn);
        using var reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            userIds.Add(reader.GetUInt32("id"));
        }

        reader.Close();

        foreach (var userId in userIds)
        {
            users.Add(UsersService.SelectById(userId, conn));
        }

        return users;
    }

    public static bool ChangeInfo(UserModelID user, MySqlConnection conn)
    {
        string changeInfoQuery =
            """
            UPDATE users
            SET username = @username,
                status_id = (SELECT id FROM statuses WHERE status = @status),
                field_of_study_id = (SELECT id FROM fields_of_study WHERE field = @field),
                specialization_id = (SELECT id FROM specializations WHERE specialization = @specialization),
                university_id = (SELECT id FROM universities WHERE university = @university),
                degree_id = (SELECT id FROM degrees WHERE degree = @degree),
                term = @term,
                email = @email,
                photo = @photo
            """;

        if (user.Password != null)
        {
            changeInfoQuery += ", password = @password";
        }

        changeInfoQuery += " WHERE id = @id";

        using var updateCommand = new MySqlCommand(changeInfoQuery, conn);
        updateCommand.Parameters.AddWithValue("@id", user.ID);
        updateCommand.Parameters.AddWithValue("@username", user.Username);
        updateCommand.Parameters.AddWithValue("@status", user.Status);
        updateCommand.Parameters.AddWithValue("@field", user.FieldOfStudy);
        updateCommand.Parameters.AddWithValue("@specialization", user.Specialization);
        updateCommand.Parameters.AddWithValue("@university", user.University);
        updateCommand.Parameters.AddWithValue("@degree", user.Degree);
        updateCommand.Parameters.AddWithValue("@term", user.Term);
        updateCommand.Parameters.AddWithValue("@email", user.Email);
        updateCommand.Parameters.AddWithValue("@photo", user.Photo);

        if (user.Password != null)
        {
            updateCommand.Parameters.AddWithValue("@password", user.Password);
        }

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static bool ChangePassword(uint id, string newPassword, MySqlConnection conn)
    {
        string changePasswordQuery =
            $"""
              UPDATE users
              SET password = {newPassword}
              WHERE id = @user_id
              """;

        using var updateCommand = new MySqlCommand(changePasswordQuery, conn);
        updateCommand.Parameters.AddWithValue("@user_id", id);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }


    public static bool Ban(uint userId, MySqlConnection conn)
    {
        string banUserQuery =
            """
            UPDATE users SET is_banned = 1 WHERE id = @userId;
            UPDATE topics SET is_deleted = TRUE WHERE user_id = @userId;
            UPDATE replies SET is_deleted = TRUE WHERE creator_id = @userId
            """;

        using var updateCommand = new MySqlCommand(banUserQuery, conn);
        updateCommand.Parameters.AddWithValue("@userId", userId);

        int affectedRows = updateCommand.ExecuteNonQuery();
        return affectedRows != 0;
    }

    public static bool Unban(uint userId, MySqlConnection conn)
    {
        string unbanUserQuery =
            """
            UPDATE users SET is_banned = FALSE WHERE id = @user_id
            """;

        using var updateCommand = new MySqlCommand(unbanUserQuery, conn);
        updateCommand.Parameters.AddWithValue("@user_id", userId);

        int affectedRows = updateCommand.ExecuteNonQuery();
        return affectedRows != 0;
    }

    public static List<UserModelID> SelectBannedUsers(MySqlConnection conn)
    {
        List<UserModelID> bannedUsers = [];
        List<uint> bannedUserIds = [];
        string selectBannedQuery =
            """
            SELECT id FROM users WHERE is_banned = TRUE
            """;

        using var selectCommand = new MySqlCommand(selectBannedQuery);
        using var reader = selectCommand.ExecuteReader();

        while (reader.Read())
        {
            bannedUserIds.Add(reader.GetUInt32("id"));
        }
        reader.Close();

        foreach (var bannedUserId in bannedUserIds)
        {
            bannedUsers.Add(SelectById(bannedUserId, conn));
        }

        return bannedUsers;
    }

    public static bool ChangePhoto(uint userId, string newPhoto, MySqlConnection conn)
    {
        string query =
            """
            UPDATE users
            SET photo = @new_photo
            WHERE id = @user_id
            """;

        using var command = new MySqlCommand(query, conn);
        command.Parameters.AddWithValue("@new_photo", newPhoto);
        command.Parameters.AddWithValue("@user_id", userId);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected != 0;
    }

    public static List<string> GetStatuses(MySqlConnection conn)
    {
        var statuses = new List<string>();

        string query = "SELECT status FROM statuses";

        using var command = new MySqlCommand(query, conn);

        using var read = command.ExecuteReader();
        while (read.Read())
        {
            statuses.Add(read["status"].ToString() ?? string.Empty);
        }

        return statuses;
    }

    public static List<string> GetFieldsOfStudy(MySqlConnection conn)
    {
        var fields = new List<string>();

        string query = "SELECT field FROM fields_of_study";

        using var command = new MySqlCommand(query, conn);

        using var read = command.ExecuteReader();
        while (read.Read())
        {
            fields.Add(read["field"].ToString() ?? string.Empty);
        }

        return fields;
    }

    public static List<string> GetSpecializations(MySqlConnection conn)
    {
        var specialization = new List<string>();

        string query = "SELECT specialization FROM specializations";

        using var command = new MySqlCommand(query, conn);

        using var read = command.ExecuteReader();
        while (read.Read())
        {
            specialization.Add(read["specialization"].ToString() ?? string.Empty);
        }

        return specialization;
    }

    public static List<string> GetUniversities(MySqlConnection conn)
    {
        var universities = new List<string>();

        string query = "SELECT university FROM universities";

        using var command = new MySqlCommand(query, conn);

        using var read = command.ExecuteReader();
        while (read.Read())
        {
            universities.Add(read["university"].ToString() ?? string.Empty);
        }

        return universities;
    }

    public static List<string> GetDegrees(MySqlConnection conn)
    {
        var degrees = new List<string>();

        string query = "SELECT degree FROM degrees";

        using var command = new MySqlCommand(query, conn);

        using var read = command.ExecuteReader();
        while (read.Read())
        {
            degrees.Add(read["degree"].ToString() ?? string.Empty);
        }

        return degrees;
    }
}