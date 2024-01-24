﻿using EleCho.GoCqHttpSdk.Message.DataModel;
using EleCho.GoCqHttpSdk.Utils;
using System;

namespace EleCho.GoCqHttpSdk.Message
{
    /// <summary>
    /// @某人
    /// </summary>
    public record class CqAtMsg : CqMsg
    {
        /// <summary>
        /// 消息类型: @
        /// </summary>
        public override string MsgType => Consts.MsgType.At;

        /// <summary>
        /// 说明: @ 的目标 QQ 号, all 表示全体成员
        /// 可能的值: QQ 号, all
        /// </summary>
        public long Target { get; set; } = -1;

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 是 AT 全体成员
        /// </summary>
        public bool IsAtAll { get; set; }

        internal CqAtMsg()
        { }


        /// <summary>
        /// 构建 @ 消息段
        /// </summary>
        /// <param name="target"></param>
        public CqAtMsg(long target)
        {
            Target = target;
        }

        /// <summary>
        /// 获取 AT 所有人的消息段
        /// </summary>
        public static CqAtMsg AtAll => new CqAtMsg() { IsAtAll = true };

        internal override void ReadDataModel(CqMsgDataModel? model)
        {
            CqAtMsgDataModel? m = model as CqAtMsgDataModel;
            if (m == null)
                throw new ArgumentException();

            if (long.TryParse(m.qq, out long _qq))
                Target = _qq;
            else if (m.qq.Equals("all", StringComparison.OrdinalIgnoreCase))
                IsAtAll = true;

            Name = m.name;
        }

        internal override CqMsgDataModel? GetDataModel()
        {
            return new CqAtMsgDataModel(IsAtAll ? "all" : Target.ToString(), Name);
        }
    }
}