﻿using EleCho.GoCqHttpSdk.Message.DataModel;
using EleCho.GoCqHttpSdk.Utils;
using System;

namespace EleCho.GoCqHttpSdk.Message
{
    /// <summary>
    /// 窗口抖动（戳一戳<br/>
    /// 相当于戳一戳最基本类型的快捷方式
    /// </summary>
    [Obsolete(CqMsg.NotSupportedCqCodeTip)]
    public record class CqShakeMsg : CqMsg
    {
        /// <summary>
        /// 消息段类型: 窗口抖动
        /// </summary>
        public override string MsgType => Consts.MsgType.Shake;

        internal override CqMsgDataModel? GetDataModel() => new CqShakeMsgDataModel();

        internal override void ReadDataModel(CqMsgDataModel? model)
        { }
    }
}