﻿#pragma warning disable IDE1006 // Naming Styles

namespace EleCho.GoCqHttpSdk.Action.Model.Params
{
    internal class CqGetForwardMessageActionParamsModel : CqActionParamsModel
    {
        public CqGetForwardMessageActionParamsModel(long message_id) => this.message_id = message_id;

        public long message_id { get; }
    }
}