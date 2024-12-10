namespace Server.Models;

public class ChildReplyModelID : ReplyModelID
{
    public uint RootReplyID { get; set; }
    public uint ReplyToUserID { get; set; }
}