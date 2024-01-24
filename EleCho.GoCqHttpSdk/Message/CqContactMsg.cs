﻿
using EleCho.GoCqHttpSdk.Message.DataModel;
using EleCho.GoCqHttpSdk.Utils;
using System;

namespace EleCho.GoCqHttpSdk.Message
{
    /// <summary>
    /// 联系人消息段 (推荐好友/群)
    /// </summary>
    [Obsolete(CqMsg.NotSupportedCqCodeTip)]
    public record class CqContactMsg : CqMsg
    {
        /// <summary>
        /// 消息段: 联系人
        /// </summary>
        public override string MsgType => Consts.MsgType.Contact;

        internal CqContactMsg()
        { }

        /// <summary>
        /// 构建联系人消息段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public CqContactMsg(CqContactType type, long id)
        {
            ContactType = type;
            Id = id;
        }

        /// <summary>
        /// 推荐好友/群
        /// </summary>
        public CqContactType ContactType { get; set; }

        /// <summary>
        /// 被推荐的 QQ （群）号
        /// </summary>
        public long Id { get; set; }

        internal override CqMsgDataModel? GetDataModel() => new CqContactMsgDataModel(CqEnum.GetString(ContactType) ?? "", Id);

        internal override void ReadDataModel(CqMsgDataModel? model)
        {
            CqContactMsgDataModel? m = model as CqContactMsgDataModel;
            if (m == null)
                throw new ArgumentException();

            ContactType = CqEnum.GetContactType(m.type);
            Id = m.id;
        }
    }
}