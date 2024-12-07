namespace Server.Models;

public class ChildReplyModel : ReplyModel
{
    public ReplyModel RootReply { get; set; }
    public UserModel ReplyTo { get; set; }
}