﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EleCho.GoCqHttpSdk.Action.Model.Params;

namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 获取 CSRF Token 操作
    /// </summary>
    public class CqGetCsrfTokenAction : CqAction
    {
        /// <summary>
        /// 操作类型: 获取 CSRF Token
        /// </summary>
        public override CqActionType ActionType => CqActionType.GetCsrfToken;

        internal override CqActionParamsModel GetParamsModel()
        {
            return new CqGetCsrfTokenActionParamsModel();
        }
    }
}
