﻿
using EleCho.GoCqHttpSdk.Post.Model;

namespace EleCho.GoCqHttpSdk.Post
{
    /// <summary>
    /// 群精华消息变更上报上下文
    /// </summary>
    public record class CqGroupEssenceChangedPostContext : CqNoticePostContext
    {
        internal CqGroupEssenceChangedPostContext() { }

        /// <summary>
        /// 通知类型: 精华消息
        /// </summary>
        public override CqNoticeType NoticeType => CqNoticeType.Essence;

        /// <summary>
        /// 变更类型
        /// </summary>
        public CqEssenceChangeType ChangeType { get; set; }

        /// <summary>
        /// 消息发送者 QQ
        /// </summary>
        public long SenderId { get; set; }

        /// <summary>
        /// 操作者 QQ
        /// </summary>
        public long OperatorId { get; set; }

        /// <summary>
        /// 消息 ID
        /// </summary>
        public long MessageId { get; set; }

        internal override object? QuickOperationModel => null;
        internal override void ReadModel(CqPostModel model)
        {
            base.ReadModel(model);

            if (model is not CqNoticeEssencePostModel noticeModel)
                return;

            ChangeType = CqEnum.GetEssenceChangeType(noticeModel.sub_type);
            SenderId = noticeModel.sender_id;
            OperatorId = noticeModel.operator_id;
            MessageId = noticeModel.message_id;
        }
    }
}