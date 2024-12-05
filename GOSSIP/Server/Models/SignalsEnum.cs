using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public enum SignalsEnum : byte
    {
        #region User
        Register,
        Login,
        Logout,
        EditUser,
        #endregion

        #region Topics
        GetTopics,
        CreateTopic,
        EditTopic,
        DeleteTopic,
        UpvoteTopic,
        DownvoteTopic,
        ReplyToTopic,
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
        DeleteChat
        #endregion
    }
}
