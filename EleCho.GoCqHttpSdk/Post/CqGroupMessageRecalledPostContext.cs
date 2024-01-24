﻿
using EleCho.GoCqHttpSdk.Post.Model;

namespace EleCho.GoCqHttpSdk.Post
{
    /// <summary>
    /// 群消息撤回上报上下文
    /// </summary>
    public record class CqGroupMessageRecalledPostContext : CqNoticePostContext
    {
        /// <summary>
        /// 通知类型
        /// </summary>
        public override CqNoticeType NoticeType => CqNoticeType.GroupRecall;

        /// <summary>
        /// 群号
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 用户 QQ
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 操作者 QQ
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 消息 ID
        /// </summary>
        public long MessageId { get; set; }

        internal CqGroupMessageRecalledPostContext() { }

        internal override object? QuickOperationModel => null;
        internal override void ReadModel(CqPostModel model)
        {
            base.ReadModel(model);

            if (model is not CqNoticeGroupRecallPostModel postModel)
                return;

            GroupId = postModel.group_id;
            UserId = postModel.user_id;
            OperatorId = postModel.operator_id;
            MessageId = postModel.message_id;
        }
    }
}