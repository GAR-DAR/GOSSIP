using System.Data;
using MySql.Data.MySqlClient;
using Server.Models;

namespace Server.Services;

// TODO: ban user
// TODO: change photo
// TODO: refactor Exists method
public static class UsersService
{
    public static bool SignUp(UserModel user, MySqlConnection conn)
    {
        if (Exists("email", user.Email, conn))
            return false; // TODO: an exception, for sure
        
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
                     @created_at, @is_banned
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
        insertCommand.Parameters.AddWithValue("@created_at", user.CreatedAt);
        insertCommand.Parameters.AddWithValue("@is_banned", user.IsBanned);

        int affectedRows = insertCommand.ExecuteNonQuery();

        return affectedRows != 0;
    }
    
    /// <param name="typeOfLogin">"email" or "username"</param>
    public static UserModel? SignIn(string typeOfLogin, string login, string password, MySqlConnection conn)
    {
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

        var user = new UserModel
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
            IsBanned = reader.GetBoolean("is_banned")
            // TODO: ChatModels
        };
        
        reader.Close();

        user.Chats = ChatsService.SelectByUser(user, conn);

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

    // TODO: wtf I've written ????
    public static UserModel Select(uint id, MySqlConnection conn)
    {
        string selectQuery =
            """
            SELECT * FROM users WHERE id = @id
            """;

        using var selectCommand = new MySqlCommand(selectQuery, conn);
        selectCommand.Parameters.AddWithValue("@id", id);

        using var reader = selectCommand.ExecuteReader();
        reader.Read();

        return new UserModel
        {
            ID = id,
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
            // TODO: ChatModels ? 
        };
    }

    public static bool ChangePassword (UserModel user, string newPassword, MySqlConnection conn)
    {
        string changePasswordQuery =
           $"""
            UPDATE users
            SET password = { newPassword }
            WHERE id = @user_id
            """;

        using var updateCommand = new MySqlCommand(changePasswordQuery, conn);
        updateCommand.Parameters.AddWithValue("@user_id", user.ID);

        int rowsAffected = updateCommand.ExecuteNonQuery();
        return rowsAffected != 0;
    }
    
}