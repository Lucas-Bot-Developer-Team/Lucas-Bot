﻿using EleCho.GoCqHttpSdk.Action.Model.Params;


namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 踢出群成员操作
    /// </summary>
    public class CqKickGroupMemberAction : CqAction
    {
        /// <summary>
        /// 操作类型: 踢群成员
        /// </summary>
        public override CqActionType ActionType => CqActionType.KickGroupMember;

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="userId">用户 QQ</param>
        public CqKickGroupMemberAction(long groupId, long userId)
            : this(groupId, userId, false)
        { }

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="userId">用户 QQ</param>
        /// <param name="rejectRequest">拒绝加群请求</param>
        public CqKickGroupMemberAction(long groupId, long userId, bool rejectRequest)
        {
            GroupId = groupId;
            UserId = userId;
            RejectRequest = rejectRequest;
        }

        /// <summary>
        /// 群 ID
        /// </summary>
        public long GroupId { get; set; }
        /// <summary>
        /// 用户 ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 拒绝该用户的加群请求
        /// </summary>
        public bool RejectRequest { get; set; }


        internal override CqActionParamsModel GetParamsModel()
        {
            return new CqKickGroupMemberActionParamsModel(GroupId, UserId, RejectRequest);
        }
    }
}