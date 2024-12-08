using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public enum SignalsEnum : byte
    {
        #region User
        Disconnect,
        SignUp,
        Login,
        Logout,
        EditUser,
        RefreshUser,
        ChangeUserPhoto,
        #endregion

        #region Topics
        GetTopics,
        CreateTopic,
        EditTopic,
        DeleteTopic,
        UpvoteTopic,
        DownvoteTopic,

        #endregion

        #region Reply
        CreateReply,
        EditReply,
        DeleteReply,
        UpvoteReply,
        DownvoteReply,
        ReplyToReply,
        #endregion

        #region Chat
        StartChat,
        SendMessage,
        DeleteMessage,
        EditMessage,
        ReadMessage,
        DeleteChat,
        #endregion

        #region Errors
        PacketError,
        LoginError,
        SignUpError,
        #endregion
    }
}
