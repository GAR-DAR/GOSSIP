using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Server.Services;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new DatabaseService();

            var topics = TopicsService.SelectAll(db.Connection);

            Console.WriteLine(JsonSerializer.Serialize(topics, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            }));
        }
    }
}
