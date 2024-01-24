﻿using EleCho.GoCqHttpSdk.Action.Model.Params;

using EleCho.GoCqHttpSdk.Utils;
using System;

namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 设置群荣誉操作
    /// </summary>
    public class CqSetGroupSpecialTitleAction : CqAction
    {
        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="userId">用户 QQ</param>
        /// <param name="specialTitle">群荣誉</param>
        public CqSetGroupSpecialTitleAction(long groupId, long userId, string specialTitle) :
            this(groupId, userId, specialTitle, TimeSpan.FromSeconds(-1))
        {
        }

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="userId">用户 QQ</param>
        /// <param name="specialTitle">群荣誉</param>
        /// <param name="duration">持续时间</param>
        public CqSetGroupSpecialTitleAction(long groupId, long userId, string specialTitle, TimeSpan duration)
        {
            UserId = userId;
            GroupId = groupId;
            SpecialTitle = specialTitle;
            Duration = duration;
        }

        /// <summary>
        /// 操作类型: 设置群荣誉
        /// </summary>
        public override CqActionType ActionType => CqActionType.SetGroupSpecialTitle;
        
        /// <summary>
        /// 群号
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 用户 QQ
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 群榕树
        /// </summary>
        public string SpecialTitle { get; set; }

        /// <summary>
        /// 持续时间
        /// </summary>
        public TimeSpan Duration { get; set; }

        internal override CqActionParamsModel GetParamsModel()
        {
            return new CqSetGroupSpecialTitleActionParamsModel(GroupId, UserId, SpecialTitle, Duration.ToLongTotalSeconds());
        }
    }
}
