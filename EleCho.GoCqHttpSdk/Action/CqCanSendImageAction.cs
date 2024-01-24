﻿using EleCho.GoCqHttpSdk.Action.Model.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 检查是否能发送图像的操作
    /// </summary>
    public class CqCanSendImageAction : CqAction
    {
        /// <summary>
        /// 操作类型: 能发送图片
        /// </summary>
        public override CqActionType ActionType => CqActionType.CanSendImage;

        internal override CqActionParamsModel GetParamsModel() => new CqCanSendImageActionParamsModel();
    }
}
