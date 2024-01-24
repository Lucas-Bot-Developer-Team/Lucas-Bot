﻿using EleCho.GoCqHttpSdk.Action.Model.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk.Action
{
    /// <summary>
    /// 设置群管理员操作结果
    /// </summary>
    public record class CqSetGroupAdministratorActionResult : CqActionResult
    {
        internal CqSetGroupAdministratorActionResult() { }

        internal override void ReadDataModel(CqActionResultDataModel? model)
        {
            // no data
        }
    }
}
