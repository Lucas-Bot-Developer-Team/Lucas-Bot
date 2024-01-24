﻿#pragma warning disable IDE1006 // Naming Styles

namespace EleCho.GoCqHttpSdk.Action.Model.Params
{
    internal class CqBanGroupMemberActionParamsModel : CqActionParamsModel
    {
        public CqBanGroupMemberActionParamsModel(long group_id, long user_id, long duration)
        {
            this.group_id = group_id;
            this.user_id = user_id;
            this.duration = duration;
        }

        public long group_id { get; }
        public long user_id { get; }
        public long duration { get; }
    }
}