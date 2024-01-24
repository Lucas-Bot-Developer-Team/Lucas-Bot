﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles

using EleCho.GoCqHttpSdk.DataStructure.Model;

namespace EleCho.GoCqHttpSdk.Post.Model
{
    internal class CqNoticeOfflineFilePostModel : CqNoticePostModel
    {
        public override string notice_type => "offline_file";

        public long user_id { get; set; }
        public CqOfflineFileModel file { get; set; }
    }
}