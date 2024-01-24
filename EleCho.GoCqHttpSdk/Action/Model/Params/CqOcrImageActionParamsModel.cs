﻿#pragma warning disable IDE1006 // Naming Styles

namespace EleCho.GoCqHttpSdk.Action.Model.Params
{
    internal class CqOcrImageActionParamsModel : CqActionParamsModel
    {
        public string image { get; }

        public CqOcrImageActionParamsModel(string image)
        {
            this.image = image;
        }
    }
}