using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Server.Models;
using Server.Services;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new DatabaseService();

            Console.WriteLine(
                JsonSerializer.Serialize(JsonSerializer.Deserialize<UserModel?>(JsonSerializer.Serialize(
                    UsersService.SignIn(
                "email", "yurii.stelmakh.pz.2023@lpnu.ua", "password", db.Connection), 
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.Preserve 
                    }
                    ),
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    }),
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.Preserve
                    })
                );
        }
    }
}
