﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EleCho.GoCqHttpSdk.DataStructure.Model;
using EleCho.GoCqHttpSdk.Utils;
using static EleCho.GoCqHttpSdk.Utils.Consts.ActionType;

namespace EleCho.GoCqHttpSdk.Action.Model.ResultData
{
    internal class CqActionResultDataModel
    {
        internal static CqActionResultDataModel? FromRaw(JsonElement? data, string actionType)
        {
            if (data == null)
                return null;

            JsonElement dataValue = data.Value;
            return actionType switch
            {
                SendPrivateMsg => dataValue.Deserialize<CqSendPrivateMessageActionResultDataModel>(JsonHelper.Options),
                SendGroupMsg => dataValue.Deserialize<CqSendGroupMessageActionResultDataModel>(JsonHelper.Options),
                SendMsg => dataValue.Deserialize<CqSendMessageActionResultDataModel>(JsonHelper.Options),
                DeleteMsg => dataValue.Deserialize<CqRecallMessageActionResultDataModel>(JsonHelper.Options),
                SendGroupForwardMsg => dataValue.Deserialize<CqSendGroupForwardMessageActionResultDataModel>(JsonHelper.Options),
                SendPrivateForwardMsg => dataValue.Deserialize<CqSendPrivateForwardMessageActionResultDataModel>(JsonHelper.Options),

                GetMsg => dataValue.Deserialize<CqGetMessageActionResultDataModel>(JsonHelper.Options),
                GetForwardMsg => dataValue.Deserialize<CqGetForwardMessageActionResultDataModel>(JsonHelper.Options),
                GetImage => dataValue.Deserialize<CqGetImageActionResultDataModel>(JsonHelper.Options),
                GetUnidirectionalFriendList => dataValue.Deserialize<CqGetUnidirectionalFriendListActionResultDataModel>(JsonHelper.Options),

                GetFriendList => dataValue.Deserialize<CqGetFriendListActionResultDataModel>(JsonHelper.Options),
                GetGroupList => dataValue.Deserialize<CqGetGroupListActionResultDataModel>(JsonHelper.Options),
                GetGroupMemberList => dataValue.Deserialize<CqGetGroupMemberListActionResultDataModel>(JsonHelper.Options),


                GetLoginInfo => dataValue.Deserialize<CqGetLoginInformationActionResultDataModel>(JsonHelper.Options),
                GetStrangerInfo => dataValue.Deserialize<CqGetStrangerInformationActionResultDataModel>(JsonHelper.Options),
                GetGroupInfo => dataValue.Deserialize<CqGetGroupInformationActionResultDataModel>(JsonHelper.Options),
                GetGroupMemberInfo => dataValue.Deserialize<CqGetGroupMemberInformationActionResultDataModel>(JsonHelper.Options),

                SetGroupBan => dataValue.Deserialize<CqBanGroupMemberActionResultDataModel>(JsonHelper.Options),
                SetGroupAnonymousBan => dataValue.Deserialize<CqBanGroupAnonymousMemberActionResultDataModel>(JsonHelper.Options),
                SetGroupWholeBan => dataValue.Deserialize<CqBanGroupAllMembersActionResultDataModel>(JsonHelper.Options),
                SetGroupKick => dataValue.Deserialize<CqKickGroupMemberActionResultDataModel>(JsonHelper.Options),

                SetFriendAddRequest => dataValue.Deserialize<CqHandleFriendRequestActionResultDataModel>(JsonHelper.Options),
                SetGroupAddRequest => dataValue.Deserialize<CqHandleGroupRequestActionResultDataModel>(JsonHelper.Options),

                MarkMsgAsRead => dataValue.Deserialize<CqMarkMessageAsReadActionResultDataModel>(JsonHelper.Options),
                SetGroupAdmin => dataValue.Deserialize<CqSetGroupAdministratorActionResultDataModel>(JsonHelper.Options),
                SetGroupAnonymous => dataValue.Deserialize<CqSetGroupAnonymousActionResultDataModel>(JsonHelper.Options),
                SetGroupCard => dataValue.Deserialize<CqSetGroupNicknameActionResultDataModel>(JsonHelper.Options),
                SetGroupName => dataValue.Deserialize<CqSetGroupNameActionResultDataModel>(JsonHelper.Options),
                SetGroupPortrait => dataValue.Deserialize<CqSetGroupAvatarActionResultDataModel>(JsonHelper.Options),
                SetGroupLeave => dataValue.Deserialize<CqLeaveGroupActionResultDataModel>(JsonHelper.Options),
                SetGroupSpecialTitle => dataValue.Deserialize<CqSetGroupSpecialTitleActionResultDataModel>(JsonHelper.Options),
                SetQqProfile => dataValue.Deserialize<CqSetAccountProfileActionResultDataModel>(JsonHelper.Options),

                SendGroupSign => dataValue.Deserialize<CqGroupSignInActionResultDataModel>(JsonHelper.Options),

                DeleteFriend => dataValue.Deserialize<CqDeleteFriendActionResultDataModel>(JsonHelper.Options),
                DeleteUnidirectionalFriend => dataValue.Deserialize<CqDeleteUnidirectionalFriendActionResultDataModel>(JsonHelper.Options),

                CanSendImage => dataValue.Deserialize<CqCanSendImageActionResultDataModel>(JsonHelper.Options),
                CanSendRecord => dataValue.Deserialize<CqCanSendRecordActionResultDataModel>(JsonHelper.Options),

                GetCookies => dataValue.Deserialize<CqGetCookiesActionResultDataModel>(JsonHelper.Options),
                GetCsrfToken => dataValue.Deserialize<CqGetCsrfTokenActionResultDataModel>(JsonHelper.Options),

                DownloadFile => dataValue.Deserialize<CqDownloadFileActionResultDataModel>(JsonHelper.Options),
                GetOnlineClients => dataValue.Deserialize<CqGetOnlineClientsActionResultDataModel>(JsonHelper.Options),

                SetEssenceMsg => dataValue.Deserialize<CqSetEssenceMessageActionResultDataModel>(JsonHelper.Options),
                DeleteEssenceMsg => dataValue.Deserialize<CqDeleteEssenceMessageActionResultDataModel>(JsonHelper.Options),
                GetEssenceMsgList => dataValue.Deserialize<CqGetEssenceMessageListActionResultDataModel>(JsonHelper.Options),

                GetModelShow => dataValue.Deserialize<CqGetModelShowActionResultDataModel>(JsonHelper.Options),
                SetModelShow => dataValue.Deserialize<CqSetModelShowActionResultDataModel>(JsonHelper.Options),

                CheckUrlSafety => dataValue.Deserialize<CqCheckUrlSafetyActionResultDataModel>(JsonHelper.Options),
                GetVersionInfo => dataValue.Deserialize<CqGetVersionInformationActionResultDataModel>(JsonHelper.Options),

                ReloadEventFilter => dataValue.Deserialize<CqReloadEventFilterActionResultDataModel>(JsonHelper.Options),
                GetWordSlices => dataValue.Deserialize<CqGetWordSlicesActionResultDataModel>(JsonHelper.Options),
                OcrImage => dataValue.Deserialize<CqOcrImageActionResultDataModel>(JsonHelper.Options),

                UploadPrivateFile => dataValue.Deserialize<CqUploadPrivateFileActionResultDataModel>(JsonHelper.Options),
                UploadGroupFile => dataValue.Deserialize<CqUploadGroupFileActionResultDataModel>(JsonHelper.Options),
                DeleteGroupFile => dataValue.Deserialize<CqDeleteGroupFileActionResultDataModel>(JsonHelper.Options),
                CreateGroupFileFolder => dataValue.Deserialize<CqCreateGroupFolderActionResultDataModel>(JsonHelper.Options),
                DeleteGroupFolder => dataValue.Deserialize<CqDeleteGroupFolderActionResultDataModel>(JsonHelper.Options),
                GetGroupFileSystemInfo => dataValue.Deserialize<CqGetGroupFileSystemInformationActionResultDataModel>(JsonHelper.Options),
                GetGroupRootFiles => dataValue.Deserialize<CqGetGroupRootFilesActionResultDataModel>(JsonHelper.Options),
                GetGroupFilesByFolder => dataValue.Deserialize<CqGetGroupFilesByFolderActionResultDataModel>(JsonHelper.Options),
                GetGroupFileUrl => dataValue.Deserialize<CqGetGroupFileUrlActionResultDataModel>(JsonHelper.Options),

                _ => throw new NotImplementedException()
            };
        }
    }
}