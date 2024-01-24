﻿using EleCho.GoCqHttpSdk.Action.Model.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 获取精华消息列表操作结果
    /// </summary>
    public record class CqGetEssenceMessageListActionResult : CqActionResult
    {
        internal CqGetEssenceMessageListActionResult()
        {
        }

        /// <summary>
        /// 消息列表
        /// </summary>
        public IReadOnlyList<CqEssenceMessage> Messages { get; private set; } = Array.Empty<CqEssenceMessage>();

        internal override void ReadDataModel(CqActionResultDataModel? model)
        {
            if (model is not CqGetEssenceMessageListActionResultDataModel _m)
                throw new Exception();

            Messages = _m.Select(v => new CqEssenceMessage(v)).ToList().AsReadOnly();
        }
    }
}
