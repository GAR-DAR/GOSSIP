﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP
{
    public enum SignalsEnum : byte
    {

        GetAllUsersMessage,

        #region User
        Disconnect,
        SignUp,
        Login,
        Logout,
        EditUser,
        RefreshUser,
        ChangeUserPhoto,
        GetAllUsers,
        #endregion

        #region SignUp
        GetStatuses,
        GetFieldsOfStudy,
        GetSpecializations,
        GetUniversities,
        GetDegrees,
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
        ReplyToReplyReply,
        #endregion

        #region Chat,
        GetUserChats,
        GetUserMessages,
        StartChat,
        SendMessage,
        DeleteMessage,
        EditMessage,
        ReadMessage,
        DeleteChat,
        MessageMulticast,
        #endregion

        #region Errors
        PacketError,
        LoginError,
        SignUpError,
        #endregion
    }
}