namespace GOSSIP
{
    public class ParentReplyModel : ReplyModel
    {
        public List<ChildReplyModel> Replies { get; set; } = [];
    }
}
