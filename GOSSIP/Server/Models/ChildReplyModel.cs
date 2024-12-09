namespace Server.Models;

public class ChildReplyModel : ReplyModel
{
    public ParentReplyModel RootReply { get; set; }
    public UserModel ReplyTo { get; set; }
}