﻿#pragma warning disable IDE1006 // Naming Styles


namespace EleCho.GoCqHttpSdk.Action.Model.Params
{
    internal class CqUploadPrivateFileActionParamsModel : CqActionParamsModel
    {
        public CqUploadPrivateFileActionParamsModel(long user_id, string file, string name)
        {
            this.user_id = user_id;
            this.file = file;
            this.name = name;
        }

        public long user_id { get; }
        public string file { get; }
        public string name { get; }
    }
}