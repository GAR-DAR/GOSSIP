namespace Server.Models;

public class ParentReplyModelID : ReplyModelID
{
    public List<uint> Replies { get; set; } = [];
}