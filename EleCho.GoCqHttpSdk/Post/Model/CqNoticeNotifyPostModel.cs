﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles

namespace EleCho.GoCqHttpSdk.Post.Model
{
    internal abstract class CqNoticeNotifyPostModel : CqNoticePostModel
    {
        public override string notice_type => "notify";
        public abstract string sub_type { get; }
    }
}