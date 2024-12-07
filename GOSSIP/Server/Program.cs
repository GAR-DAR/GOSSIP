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


            var reply = new ChildReplyModel
            {
                User = new UserModel
                {
                    ID = 9
                },
                Topic = new TopicModel
                {
                    ID = 10
                },
                Content = "Хехе",
                CreatedAt = DateTime.Now,
                RootReply = new ParentReplyModel
                {
                    ID = 4,
                    User = new UserModel
                    {
                        ID = 5
                    }
                }
            };

            RepliesService.Add(reply, db.Connection);
        }
    }
}
